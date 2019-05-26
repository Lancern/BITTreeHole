using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BITTreeHole.Services.Implementations
{
    /// <summary>
    /// 提供基于 mock 数据的 <see cref="IWechatApiService"/> 实现。该实现应仅在开发环境中使用。
    /// </summary>
    internal sealed class MockWechatApiService : IWechatApiService
    {
        private static readonly string[] MockOpenIds = {
            "Mock1"
        };

        private const string MockAccessToken = "MOCK_ACCESS_TOKEN";
        private const int MockExpireTime = 30 * 24 * 60 * 60;    // One month
        private const string MockRefreshToken = "MOCK_REFRESH_TOKEN";
        private const string MockScopes = "MOCK_SCOPE";

        private string GenerateMockOpenId()
        {
            return MockOpenIds[new Random().Next(MockOpenIds.Length)];
        }

        private WechatToken CreateMockToken()
        {
            var mockJsonObject = new JObject();
            mockJsonObject["access_token"] = MockAccessToken;
            mockJsonObject["expires_in"] = MockExpireTime;
            mockJsonObject["refresh_token"] = MockRefreshToken;
            mockJsonObject["openid"] = GenerateMockOpenId();
            mockJsonObject["scope"] = MockScopes;

            return WechatToken.FromWechatJson(mockJsonObject);
        }
        
        /// <summary>
        /// 根据给定的微信访问代码创建基于 mock 数据的 <see cref="WechatToken"/> 对象。
        /// </summary>
        /// <param name="wechatCode">微信访问代码。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="wechatCode"/>为null</exception>
        public Task<WechatToken> GetWechatToken(string wechatCode)
        {
            if (wechatCode == null)
                throw new ArgumentNullException(nameof(wechatCode));

            return Task.FromResult(CreateMockToken());
        }
    }
}
