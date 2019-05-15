using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Data.Entities;
using BITTreeHole.Filters;
using BITTreeHole.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BITTreeHole.Controllers
{
    [ApiController]
    [Route("regions")]
    public sealed class RegionsController : ControllerBase
    {
        private const int IconFileSizeLimit = 100 * 1024; // 100 KiB
        
        private readonly IDataFacade _dataFacade;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(IDataFacade dataFacade,
                                 ILogger<RegionsController> logger)
        {
            _dataFacade = dataFacade ?? throw new ArgumentNullException(nameof(dataFacade));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // GET: /regions
        [HttpGet]
        [RequireJwt]
        public async Task<ActionResult<IEnumerable<RegionInfo>>> Get()
        {
            var regions = await _dataFacade.PostRegions.ToListAsync();
            return new ActionResult<IEnumerable<RegionInfo>>(
                regions.Select(entity => new RegionInfo(entity)));
        }

        // POST: /regions/{name}
        [HttpPost("{name}")]
        [RequireJwt(RequireAdmin = true)]
        public async Task<ActionResult> Post(string name, List<IFormFile> uploadedFiles)
        {
            var region = new PostRegionEntity { Title = name };
            if (uploadedFiles.Count > 0)
            {
                if (uploadedFiles.Count > 1)
                {
                    // 上传了超过一个图标文件
                    return BadRequest();
                }

                var iconFile = uploadedFiles[0];
                if (iconFile.Length > IconFileSizeLimit)
                {
                    // 图标大小超过限制
                    return BadRequest();
                }

                using (var memStream = new MemoryStream())
                {
                    await iconFile.CopyToAsync(memStream);
                    region.IconData = memStream.ToArray();
                }
            }
            
            try
            {
                _dataFacade.AddPostRegion(region);
                await _dataFacade.CommitChanges();
            }
            catch (DataFacadeException)
            {
                // name 字段不合法
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "向数据源添加板块时抛出异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }

        // GET: /regions/{id}/icon
        [HttpGet("{id}/icon")]
        [RequireJwt]
        public async Task<ActionResult> GetIcon(int id)
        {
            var entity = await _dataFacade.PostRegions
                                          .AsNoTracking()
                                          .Include(e => e.IconData)
                                          .FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return NotFound();

            return new FileStreamResult(new MemoryStream(entity.IconData ?? new byte[0]), "image/jpeg");
        }

        [HttpDelete("{id}")]
        [RequireJwt(RequireAdmin = true)]
        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _dataFacade.PostRegions
                                          .FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                // 指定的板块不存在
                return NotFound();
            }

            try
            {
                _dataFacade.RemovePostRegion(entity);
                await _dataFacade.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从数据源删除帖子板块时抛出异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            return Ok();
        }
    }
}
