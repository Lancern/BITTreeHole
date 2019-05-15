using System;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Data.Entities;
using BITTreeHole.Models;
using BITTreeHole.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BITTreeHole.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IWechatApiService _wechatApi;
        private readonly IJwtService _jwt;
        private readonly IDataFacade _dataFacade;

        private readonly ILogger<AuthController> _logger;

        public AuthController(IWechatApiService wechatApiService, 
                              IJwtService jwtService,
                              IDataFacade dataFacade,
                              ILogger<AuthController> logger)
        {
            _wechatApi = wechatApiService;
            _jwt = jwtService;
            _dataFacade = dataFacade;
            _logger = logger;
        }

        // POST: /auth
        [HttpPost]
        public async Task<ActionResult<AuthenticationResult>> Post([FromBody] LoginInfo loginInfo)
        {
            WechatToken wechatToken;
            try
            {
                wechatToken = await _wechatApi.GetWechatToken(loginInfo.WechatCode);
            }
            catch (WechatApiException ex)
            {
                return AuthenticationResult.Failure("无效的 Wechat code");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通过微信 API 获取 access_code 时发生异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }

            UserEntity userEntity;
            try
            {
                userEntity = await _dataFacade.AddOrFindUserByWechatId(wechatToken.OpenId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据微信 ID 向数据源插入或查找用户数据时抛出异常：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            
            var token = new AuthenticationToken(userEntity.Id, wechatToken);
            return AuthenticationResult.Success(_jwt.Encode(token));
        }
    }
}
