using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BITTreeHole.Services.Implementations
{
    /// <summary>
    /// 提供 <see cref="IWechatApiService"/> 接口的默认实现。
    /// </summary>
    internal sealed class DefaultWechatApiService : IWechatApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DefaultWechatApiService> _logger;
        private readonly WechatApiServiceOptions _options;

        /// <summary>
        /// 初始化 <see cref="DefaultWechatApiService"/> 类的新实例。
        /// </summary>
        /// <param name="options">服务配置对象。</param>
        /// <param name="httpClientFactory">HTTP客户端工厂对象。</param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientFactory"/>为null。</exception>
        public DefaultWechatApiService(WechatApiServiceOptions options, 
                                       IHttpClientFactory httpClientFactory,
                                       ILogger<DefaultWechatApiService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// 向给定的 URL 发送 GET 请求并获取响应。
        /// </summary>
        /// <param name="url">目标URL。</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetApiResponse(string url)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                return await httpClient.GetAsync(url);
            }
        }

        /// <inheritdoc />
        public async Task<WechatToken> GetWechatToken(string wechatCode)
        {
            if (wechatCode == null)
                throw new ArgumentNullException(nameof(wechatCode));

            var url = string.Format(
                "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}",
                _options.AppId, _options.AppSecret, wechatCode, "authorization_code");

            HttpResponseMessage response;
            try
            {
                response = await GetApiResponse(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "尝试访问微信 API 时发生异常：{0}: {1}", ex.GetType(), ex.Message);
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("微信 API 返回非 2XX 的 HTTP 状态码：{0}", response.StatusCode);
                // TODO: 更改下面的异常类型
                throw new Exception($"微信 API 返回了非 2XX 的 HTTP 状态码：{response.StatusCode}");
            }

            string responseBody;
            try
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "尝试读取微信 API 响应时发生异常：{0}: {1}", ex.GetType(), ex.Message);
                throw;
            }

            var responseJson = JObject.Parse(responseBody);
            var errorCode = responseJson["errcode"]?.Value<int>();
            if (errorCode != null && errorCode != 0)
            {
                var errorMessage = (string) responseJson["errmsg"];
                throw new WechatApiException(errorCode.Value, errorMessage);
            }

            return WechatToken.FromWechatJson(responseJson);
        }
    }
}
