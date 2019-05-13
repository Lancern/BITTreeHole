using System;
using System.Runtime.Serialization;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 表示当数据源返回错误时抛出的异常。
    /// </summary>
    [Serializable]
    public class DataFacadeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 初始化 <see cref="DataFacadeException"/> 类的新实例。
        /// </summary>
        public DataFacadeException()
        {
        }

        /// <summary>
        /// 初始化 <see cref="DataFacadeException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public DataFacadeException(string message) : base(message)
        {
        }

        /// <summary>
        /// 初始化 <see cref="DataFacadeException"/> 类的新实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="inner">引发当前异常的内部异常。</param>
        public DataFacadeException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 当需要从序列化环境中反序列化 <see cref="DataFacadeException"/> 时被调用。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DataFacadeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
