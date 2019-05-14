using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 封装访问微信 API 所需的授权信息。
    /// </summary>
    public sealed class WechatToken
    {
        /// <summary>
        /// 为 <see cref="WechatToken"/> 提供从微信端 JSON 反序列化的逻辑。
        /// </summary>
        private sealed class WechatTokenJsonConverter : JsonConverter<WechatToken>
        {
            /// <summary>
            /// 该方法不应该被调用。恒抛出 <see cref="Exception"/>。
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="serializer"></param>
            /// <exception cref="Exception"></exception>
            public override void WriteJson(JsonWriter writer, WechatToken value, JsonSerializer serializer)
            {
                throw new Exception("不能使用 WechatTokenJsonConverter 对 WechatToken 进行序列化。请使用默认的 JsonConverter。");
            }

            /// <summary>
            /// 从给定的 <see cref="JsonReader"/> 中读取 <see cref="WechatToken"/> 对象。
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="objectType"></param>
            /// <param name="existingValue"></param>
            /// <param name="hasExistingValue"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override WechatToken ReadJson(JsonReader reader, Type objectType, WechatToken existingValue, 
                                                 bool hasExistingValue, JsonSerializer serializer)
            {
                var jsonRawObject = (JObject)JToken.ReadFrom(reader);
                return WechatToken.FromWechatJson(jsonRawObject);
            }
        }
        
        /// <summary>
        /// 获取微信的 access_token。
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; private set; }
        
        /// <summary>
        /// 获取 access_token 的过期时间。
        /// </summary>
        [JsonProperty("expireTime")]
        public DateTime ExpireTime { get; private set; }

        /// <summary>
        /// access_token 是否已经过期。
        /// </summary>
        [JsonIgnore]
        public bool HasExpired => DateTime.Now >= ExpireTime;
        
        /// <summary>
        /// 获取用于刷新 access_token 的 refresh_token。
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; private set; }
        
        /// <summary>
        /// 获取微信用户的应用全局唯一标识。
        /// </summary>
        [JsonProperty("openId")]
        public string OpenId { get; private set; }

        /// <summary>
        /// 获取用户授权的作用域。
        /// </summary>
        [JsonProperty("scopes")]
        public string[] Scopes { get; private set; }
        
        /// <summary>
        /// 获取用户身份信息 ID。
        /// </summary>
        [JsonProperty("unionId")]
        public string UnionId { get; private set; }

        /// <summary>
        /// 从给定的 JSON 片段中反序列化 <see cref="WechatToken"/> 对象的实例。
        /// </summary>
        /// <param name="json">微信 API 端返回的 JSON 序列。</param>
        /// <returns>从给定的 JSON 序列中反序列化的 <see cref="WechatToken"/> 对象。</returns>
        /// <exception cref="ArgumentNullException">json 为 null。</exception>
        public static WechatToken FromWechatJson(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            
            return JsonConvert.DeserializeObject<WechatToken>(json, new WechatTokenJsonConverter());
        }

        /// <summary>
        /// 从给定的 <see cref="JObject"/> 反序列化 <see cref="WechatToken"/> 对象的实例。
        /// </summary>
        /// <param name="jsonObject">微信 API 端返回的 JSON 序列的 <see cref="JObject"/> 表示。</param>
        /// <returns>反序列化出的 <see cref="WechatToken"/> 对象。</returns>
        /// <exception cref="ArgumentNullException">jsonObject 为 null。</exception>
        public static WechatToken FromWechatJson(JObject jsonObject)
        {
            if (jsonObject == null)
                throw new ArgumentNullException(nameof(jsonObject));
            
            var wechatToken = new WechatToken();
            wechatToken.AccessToken = (string) jsonObject["access_token"];
            wechatToken.ExpireTime = DateTime.Now.AddSeconds((int) jsonObject["expires_in"]);
            wechatToken.RefreshToken = (string) jsonObject["refresh_token"];
            wechatToken.Scopes = ((string) jsonObject["scope"]).Split(',');
            wechatToken.OpenId = (string) jsonObject["openid"];
            wechatToken.UnionId = (string) jsonObject["unionid"];

            return wechatToken;
        }
    }
}
