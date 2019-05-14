using System;
using System.Runtime.Serialization;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 当 Wechat API 返回错误时抛出。
    /// </summary>
    [Serializable]
    public sealed class WechatApiException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 初始化 <see cref="WechatApiException"/> 类的新实例。
        /// </summary>
        /// <param name="errorCode">微信 API 返回的错误代码。</param>
        public WechatApiException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 初始化 <see cref="WechatApiException"/> 类的新实例。
        /// </summary>
        /// <param name="errorCode">微信 API 返回的错误代码。</param>
        /// <param name="errorMessage">微信 API 返回的错误消息。</param>
        public WechatApiException(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 从反序列化环境中反序列化 <see cref="WechatApiException"/> 类的新实例。
        /// </summary>
        /// <param name="info">对象序列化信息。</param>
        /// <param name="context">对象序列化流上下文。</param>
        private WechatApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            ErrorCode = info.GetInt32(nameof(ErrorCode));
            ErrorMessage = info.GetString(nameof(ErrorMessage));
        }
        
        /// <summary>
        /// 获取微信 API 返回的错误代码。
        /// </summary>
        public int ErrorCode { get; }
        
        /// <summary>
        /// 获取微信 API 返回的错误消息。
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 将当前对象序列化到给定的序列化环境中。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">序列化上下文。</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(ErrorMessage), ErrorMessage);
        }
    }
}
