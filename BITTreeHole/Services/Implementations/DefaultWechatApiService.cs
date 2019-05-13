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

        /// <summary>
        /// 初始化 <see cref="DefaultWechatApiService"/> 类的新实例。
        /// </summary>
        /// <param name="httpClientFactory">HTTP客户端工厂对象。</param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientFactory"/>为null。</exception>
        public DefaultWechatApiService(IHttpClientFactory httpClientFactory,
                                       ILogger<DefaultWechatApiService> logger)
        {
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
        public async Task<bool> CheckAccessCodeValidity(string wechatId, string wechatAccessCode)
        {
            if (wechatId == null)
                throw new ArgumentNullException(nameof(wechatId));
            if (wechatAccessCode == null)
                throw new ArgumentNullException(nameof(wechatAccessCode));

            // 构建接口URL。
            var url = string.Format("https://api.weixin.qq.com/sns/auth?openid={0}&access_token={1}",
                                    wechatId, wechatAccessCode);

            // 发送请求并接收响应
            var response = await GetApiResponse(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("微信接口 /sns/auth 返回异常的HTTP状态码：{0}", response.StatusCode);
                return false;
            }
            
            // 检查微信API响应内容
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseBodyStructured = JObject.Parse(responseBody);

            if (!responseBodyStructured.TryGetValue("errcode", out var errorCodeField))
            {
                return false;
            }

            return errorCodeField.Type == JTokenType.Integer && errorCodeField.Value<int>() == 0;
        }
    }
}
