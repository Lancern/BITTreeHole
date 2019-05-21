using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
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

        // POST: /posts
        [HttpPost]
        [RequireJwt]
        public async Task<ActionResult<PostCreationResult>> Post([FromBody] PostInfo info)
        {
            var authToken = Request.HttpContext.GetAuthenticationToken();
            var (indexEntity, contentEntity) = _entityFactory.CreatePostEntities(authToken.UserId, info);

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
        [HttpPost("posts/{id}/images/{mask}")]
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
        [HttpDelete("posts/{id}/images/{mask}")]
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

        [HttpPut("/posts/{id}")]
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
        [HttpDelete("posts/{id}")]
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
    }
}
