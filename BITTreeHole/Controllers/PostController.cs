using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Data.Entities;
using BITTreeHole.Extensions;
using BITTreeHole.Filters;
using BITTreeHole.Models;
using BITTreeHole.Services;
using BITTreeHole.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace BITTreeHole.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IDataFacade _dataFacade;
        private readonly IEntityFactory _entityFactory;
        private readonly ILogger<PostController> _logger;

        public PostController(IDataFacade dataFacade, IEntityFactory entityFactory, ILogger<PostController> logger)
        {
            _dataFacade = dataFacade ?? throw new ArgumentNullException(nameof(dataFacade));
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // GET: /posts
        [HttpGet]
        [RequireJwt]
        public async Task<ActionResult<IEnumerable<PostListItem>>> Get(
            [FromQuery] int region,
            [FromQuery] int? page,
            [FromQuery] int? itemsPerPage)
        {
            if (page == null)
            {
                page = 0;
            }

            if (itemsPerPage == null)
            {
                itemsPerPage = int.MaxValue;
            }

            if (page < 0 || itemsPerPage <= 0)
            {
                return BadRequest();
            }

            var aggregatedEntities = await _dataFacade.FindPosts(region, page.Value, itemsPerPage.Value);
            return new ActionResult<IEnumerable<PostListItem>>(
                aggregatedEntities.Select(ep => new PostListItem(ep.IndexEntity, ep.ContentEntity)));
        }

        // GET: /posts/{id}
        [HttpGet("{id}")]
        [RequireJwt]
        public async Task<ActionResult<PostInfo>> GetPost(int id)
        {
            PostEntity indexEntity;
            PostContentEntity contentEntity;
            try
            {
                (indexEntity, contentEntity) = await _dataFacade.FindPost(id);
            }
            catch (PostNotFoundException)
            {
                // 指定的帖子未找到
                return NotFound();
            }
            catch (DataFacadeException ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            return new PostInfo(indexEntity, contentEntity);
        }

        // POST: /posts
        [HttpPost]
        [RequireJwt]
        public async Task<ActionResult<PostCreationResult>> Post([FromBody] PostCreationInfo creationInfo)
        {
            var authToken = Request.HttpContext.GetAuthenticationToken();
            var (indexEntity, contentEntity) = _entityFactory.CreatePostEntities(authToken.UserId, creationInfo);

            try
            {
                var postId = await _dataFacade.AddPost(indexEntity, contentEntity);
                return PostCreationResult.Success(postId);
            }
            catch (DataFacadeException)
            {
                return PostCreationResult.Failure("指定的帖子板块不存在。");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "访问数据源时发生异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 筛查当前用户对给定的帖子的编辑能力。
        /// </summary>
        /// <param name="postId">目标帖子。</param>
        /// <returns>
        /// 若当前用户能够编辑目标帖子，返回 null；否则返回表示相应错误的响应动作包装。
        /// </returns>
        [NonAction]
        private async Task<ActionResult> CheckUserEditionToPost(int postId)
        {
            var authToken = HttpContext.GetAuthenticationToken();
            if (authToken == null)
            {
                return Forbid();
            }

            if (authToken.IsAdmin)
            {
                // 管理员可以修改所有帖子
                return null;
            }
            
            // 检查帖子是否由当前用户发出
            var postAuthorId = 0;
            try
            {
                postAuthorId = await _dataFacade.GetPostAuthorId(postId);
            }
            catch (PostNotFoundException)
            {
                // 帖子不存在
                return NotFound();
            }

            if (postAuthorId != authToken.UserId)
            {
                // 帖子不是由当前用户发出
                return Forbid();
            }

            return null;
        }

        // POST: /posts/{id}/images/{mask}
        [HttpPost("{id}/images/{mask}")]
        [RequireJwt]
        public async Task<ActionResult> PostImages(int id, string mask, List<IFormFile> imageFiles)
        {
            Dictionary<int, IFormFile> images;
            try
            {
                images = ImageMaskUtil.ZipImageIdMask(mask, imageFiles);
            }
            catch (InvalidImageMaskException)
            {
                return BadRequest();
            }

            var ability = await CheckUserEditionToPost(id);
            if (ability != null)
            {
                return ability;
            }
            
            var imageDataStreamFactories = new Dictionary<int, Func<Stream>>();
            foreach (var (index, file) in images)
            {
                imageDataStreamFactories.Add(index, () => file.OpenReadStream());
            }

            try
            {
                await _dataFacade.UpdatePostImages(id, imageDataStreamFactories);
            }
            catch (PostNotFoundException)
            {
                // 帖子不存在。
                return NotFound();
            }
            catch (DataFacadeException ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }

        // DELETE: /posts/{id}/images/{mask}
        [HttpDelete("{id}/images/{mask}")]
        [RequireJwt]
        public async Task<ActionResult> DeleteImages(int id, string mask)
        {
            int[] imageIndexes;
            try
            {
                imageIndexes = ImageMaskUtil.ExtractImageIdFromMask(mask);
            }
            catch (InvalidImageMaskException)
            {
                // 无效的图像掩码
                return BadRequest();
            }

            var ability = await CheckUserEditionToPost(id);
            if (ability != null)
            {
                return ability;
            }

            try
            {
                await _dataFacade.RemovePostImages(id, imageIndexes);
            }
            catch (PostNotFoundException)
            {
                // 帖子不存在。
                return NotFound();
            }
            catch (DataFacadeException ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            return Ok();
        }

        // PUT: /posts/{id}
        [HttpPut("{id}")]
        [RequireJwt]
        public async Task<ActionResult> Put(int id, [FromBody] PostModificationInfo info)
        {
            // 检查用户是否有权修改帖子内容
            var ability = await CheckUserEditionToPost(id);
            if (ability != null)
            {
                return ability;
            }

            if (!info.Title.IsValueCreated && !info.Text.IsValueCreated)
            {
                return Ok();
            }

            var indexEntity = await _dataFacade.Posts
                                               .Where(e => e.Id == id && e.IsRemoved == false)
                                               .FirstOrDefaultAsync();
            if (indexEntity == null)
            {
                // 帖子不存在
                return NotFound();
            }

            // 更新上次修改时间
            indexEntity.UpdateTime = DateTime.Now;
            if (info.Title.IsValueCreated)
            {
                indexEntity.Title = info.Title.Value;
            }
            
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            if (info.Text.IsValueCreated)
            {
                var contentId = new ObjectId(indexEntity.ContentId);
                try
                {
                    await _dataFacade.UpdatePostContentText(contentId, info.Text.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: /posts/{id}
        [HttpDelete("{id}")]
        [RequireJwt]
        public async Task<ActionResult> Delete(int id)
        {
            // 检查用户是否能够修改指定的帖子
            var ability = await CheckUserEditionToPost(id);
            if (ability != null)
            {
                return ability;
            }

            try
            {
                await _dataFacade.RemovePost(id);
            }
            catch (PostNotFoundException)
            {
                // 指定的帖子未找到
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }
        
        
        // GET: /posts/{id}/comments
        [HttpGet("{id}/comments")]
        [RequireJwt]
        public async Task<ActionResult<List<PostCommentInfo>>> GetPostComments(int id)
        {
            List<(CommentEntity IndexEntity, CommentContentEntity ContentEntity)> comments;
            try
            {
                comments = await _dataFacade.FindPostComments(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            // 逆扁平化
            var rootComments = new List<PostCommentInfo>();
            var rootCommentIdDict = new Dictionary<int, int>();
            foreach (var (indexEntity, contentEntity) in comments)
            {
                if (indexEntity.PostId == null)
                {
                    continue;
                }
                
                rootComments.Add(new PostCommentInfo(indexEntity, contentEntity));
                rootCommentIdDict.Add(indexEntity.Id, rootComments.Count - 1);
            }

            foreach (var (indexEntity, contentEntity) in comments)
            {
                if (indexEntity.CommentId == null)
                {
                    continue;
                }

                if (!rootCommentIdDict.TryGetValue(indexEntity.CommentId.Value, out var rootOffset))
                {
                    continue;
                }
                
                rootComments[rootOffset].Comments.Add(new PostCommentInfo(indexEntity, contentEntity));
            }
            
            return rootComments;
        }

        // POST: /posts/{id}/comments
        [HttpPost("{id}/comments")]
        [RequireJwt]
        public async Task<ActionResult> PostComment(int id, [FromQuery] int? parentId, 
                                                    [FromBody] CommentCreationInfo creationInfo)
        {
            var authToken = HttpContext.GetAuthenticationToken();
            if (authToken == null)
            {
                return Forbid();
            }
            
            // 检查帖子是否存在
            var postEntity = await _dataFacade.Posts
                                              .FirstOrDefaultAsync(e => e.Id == id && e.IsRemoved == false);
            if (postEntity == null)
            {
                // 帖子不存在
                return NotFound();
            }

            CommentEntity indexEntity;
            CommentContentEntity contentEntity = CommentContentEntity.Create(creationInfo.Text);
            if (parentId == null)
            {
                indexEntity = CommentEntity.CreateLv1(authToken.UserId, contentEntity.Id.ToByteArray(), id);
            }
            else
            {
                // 检查父评论是否存在
                var commentsCount = await _dataFacade.Comments
                                                     .CountAsync(e => e.Id == parentId.Value && e.IsRemoved == false);
                if (commentsCount == 0)
                {
                    // 父评论不存在
                    return NotFound();
                }
                
                indexEntity = CommentEntity.CreateLv2(authToken.UserId, contentEntity.Id.ToByteArray(), parentId.Value);
            }

            try
            {
                await _dataFacade.AddPostComment(indexEntity, contentEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            // 更新帖子实体对象
            postEntity.UpdateTime = DateTime.Now;
            postEntity.NumberOfComments++;
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }

        [NonAction]
        private async Task<ActionResult> CheckUserEditionToComment(int commentId)
        {
            var authToken = HttpContext.GetAuthenticationToken();
            if (authToken.IsAdmin)
            {
                return null;
            }

            var indexEntity = await _dataFacade.Comments
                                               .Include(e => e.AuthorId)
                                               .FirstOrDefaultAsync(e => e.Id == commentId && e.IsRemoved == false);
            if (indexEntity == null)
            {
                // 帖子实体对象不存在
                return NotFound();
            }

            if (authToken.UserId != indexEntity.AuthorId)
            {
                // 非管理员用户尝试修改评论
                return Forbid();
            }

            return null;
        }

        [HttpDelete("{postId}/comments/{commentId}")]
        [RequireJwt]
        public async Task<ActionResult> DeleteComment(int postId, int commentId)
        {
            // 检查用户是否有权对帖子进行修改
            var ability = await CheckUserEditionToComment(commentId);
            if (ability != null)
            {
                return ability;
            }

            var postEntity = await _dataFacade.Posts
                                              .FirstOrDefaultAsync(e => e.Id == postId && e.IsRemoved == false);
            if (postEntity == null)
            {
                // 帖子不存在
                return NotFound();
            }

            var indexEntity = await _dataFacade.Comments
                                               .FirstOrDefaultAsync(
                                                   e => e.CommentId == commentId && e.IsRemoved == false);
            if (indexEntity == null)
            {
                // 评论不存在
                return NotFound();
            }

            indexEntity.IsRemoved = true;
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            // 更新帖子对象
            postEntity.NumberOfComments--;
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }
        

        // POST: /posts/{id}/votes
        [HttpPost("{id}/votes")]
        [RequireJwt]
        public async Task<ActionResult> PostVote(int id)
        {
            var authToken = HttpContext.GetAuthenticationToken();
            if (authToken == null)
            {
                return Forbid();
            }

            var indexEntity = await _dataFacade.Posts
                                               .FirstOrDefaultAsync(e => e.Id == id && e.IsRemoved == false);
            if (indexEntity == null)
            {
                // 未找到指定的帖子
                return NotFound();
            }

            var voteEntity = UserVotePostEntity.Create(authToken.UserId, id);
            _dataFacade.AddVoteEntity(voteEntity);

            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (DataFacadeException)
            {
                // 已经存在一个点赞
                return Ok();
            }

            indexEntity.NumberOfVotes++;
            indexEntity.UpdateTime = DateTime.Now;
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }

        // DELETE: /posts/{id}/values
        [HttpDelete("{id}/votes")]
        [RequireJwt]
        public async Task<ActionResult> DeleteVote(int id)
        {
            var authToken = HttpContext.GetAuthenticationToken();
            if (authToken == null)
            {
                return Forbid();
            }

            var indexEntity = await _dataFacade.Posts
                                               .FirstOrDefaultAsync(e => e.Id == id && e.IsRemoved == false);
            if (indexEntity == null)
            {
                // 帖子未找到
                return NotFound();
            }

            var voteEntity = await _dataFacade.UserVotePosts
                                              .FirstOrDefaultAsync(e => e.PostId == id && e.UserId == authToken.UserId);
            if (voteEntity == null)
            {
                // 点赞不存在
                return Ok();
            }

            --indexEntity.NumberOfVotes;
            indexEntity.UpdateTime = DateTime.Now;
            _dataFacade.RemoveVoteEntity(voteEntity);
            try
            {
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据源抛出了未经处理的异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }
    }
}
