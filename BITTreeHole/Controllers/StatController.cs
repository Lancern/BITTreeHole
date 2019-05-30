using System;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Extensions;
using BITTreeHole.Filters;
using BITTreeHole.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BITTreeHole.Controllers
{
    [ApiController]
    [Route("stat")]
    public class StatController : ControllerBase
    {
        private readonly IDataFacade _dataFacade;

        public StatController(IDataFacade dataFacade)
        {
            _dataFacade = dataFacade ?? throw new ArgumentNullException(nameof(dataFacade));
        }
        
        // GET: /stat
        [HttpGet]
        [RequireJwt]
        public async Task<ActionResult<UserStatisticsInfo>> Get()
        {
            var authToken = HttpContext.GetAuthenticationToken();

            var posts = await _dataFacade.Posts
                                         .Where(e => e.AuthorId == authToken.UserId && e.IsRemoved == false)
                                         .ToListAsync();
            var votes = posts.Select(e => e.NumberOfVotes)
                             .Sum();
            return new UserStatisticsInfo(authToken.UserId, posts.Count, votes);
        }
    }
}
