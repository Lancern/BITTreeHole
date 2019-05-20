using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Extensions;
using BITTreeHole.Filters;
using BITTreeHole.Models;
using BITTreeHole.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    }
}
