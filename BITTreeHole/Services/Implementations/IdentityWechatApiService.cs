using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BITTreeHole.Services.Implementations
{
    /// <summary>
    /// 提供将给定的 Wechat Code 作为 OpenID 的 <see cref="IWechatApiService"/> 的实现。
    /// </summary>
    internal sealed class IdentityWechatApiService : IWechatApiService
    {
        /// <inheritdoc />
        public Task<WechatToken> GetWechatToken(string wechatCode)
        {
            if (wechatCode == null)
                throw new ArgumentNullException(nameof(wechatCode));
            
            var mockJsonObject = new JObject();
            mockJsonObject["access_token"] = string.Empty;
            mockJsonObject["expires_in"] = 30 * 24 * 60 * 60;
            mockJsonObject["refresh_token"] = string.Empty;
            mockJsonObject["openid"] = wechatCode;
            mockJsonObject["scope"] = string.Empty;

            return Task.FromResult(WechatToken.FromWechatJson(mockJsonObject));
        }
    }
}
