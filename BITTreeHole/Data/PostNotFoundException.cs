using System;
using System.Runtime.Serialization;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 表示帖子未找到异常。
    /// </summary>
    [Serializable]
    public sealed class PostNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 初始化 <see cref="PostNotFoundException"/> 类的新实例。
        /// </summary>
        public PostNotFoundException()
        {
        }

        /// <summary>
        /// 初始化 <see cref="PostNotFoundException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public PostNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// 初始化 <see cref="PostNotFoundException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="inner">引发当前异常的内部异常。</param>
        public PostNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 从序列化上下文中反序列化 <see cref="PostNotFoundException"/> 类的实例对象。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">序列化环境的流上下文。</param>
        private PostNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
