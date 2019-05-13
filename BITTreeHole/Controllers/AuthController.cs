using System;
using System.Threading.Tasks;
using BITTreeHole.Data;
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
            // 验证给定的微信访问代码是否有效。
            var validWechatData = false;
            try
            {
                validWechatData =
                    await _wechatApi.CheckAccessCodeValidity(loginInfo.WechatId, loginInfo.WechatAccessCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查微信访问代码有效性时捕获到异常：{0}: {1}", ex.GetType(), ex.Message);
            }

            if (!validWechatData)
            {
                return AuthenticationResult.Failure();
            }
            
            // 客户端持有有效的微信访问代码。
            // 从数据层拿用户实体对象，创建用户身份标识并编码为 JWT ，最后通过模型层返回
            var userEntity = await _dataFacade.AddOrFindUserByWechatId(loginInfo.WechatId);
            var authToken = new AuthenticationToken(userEntity.Id);
            var authTokenJwt = _jwt.Encode(authToken);

            return AuthenticationResult.Success(userEntity.IsFresh, authTokenJwt);
        }
    }
}
