using System;
using System.Runtime.Serialization;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 表示评论未找到异常。
    /// </summary>
    [Serializable]
    public sealed class CommentNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 初始化 <see cref="CommentNotFoundException"/> 类的新实例。
        /// </summary>
        public CommentNotFoundException()
        {
        }

        /// <summary>
        /// 初始化 <see cref="CommentNotFoundException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public CommentNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// 初始化 <see cref="CommentNotFoundException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="inner">引发当前异常的内部异常。</param>
        public CommentNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 从给定的序列化环境中反序列化 <see cref="CommentNotFoundException"/> 类的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">序列化环境的流上下文。</param>
        private CommentNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
