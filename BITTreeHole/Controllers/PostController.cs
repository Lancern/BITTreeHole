using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Filters;
using BITTreeHole.Models;
using Microsoft.AspNetCore.Mvc;

namespace BITTreeHole.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IDataFacade _dataFacade;

        public PostController(IDataFacade dataFacade)
        {
            _dataFacade = dataFacade ?? throw new ArgumentNullException(nameof(dataFacade));
        }
        
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
    }
}
