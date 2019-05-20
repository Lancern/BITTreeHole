using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为创建帖子操作提供结果数据模型。
    /// </summary>
    public sealed class PostCreationResult
    {
        /// <summary>
        /// 获取或设置操作是否成功。
        /// </summary>
        [JsonProperty("succeed")]
        public bool Succeed { get; set; }
        
        /// <summary>
        /// 获取或设置操作产生的消息。
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
        
        /// <summary>
        /// 获取或设置新创建的帖子 ID。
        /// </summary>
        [JsonProperty("postId")]
        public int PostId { get; set; }

        /// <summary>
        /// 创建表示操作成功完成的 <see cref="PostCreationResult"/> 对象。
        /// </summary>
        /// <param name="postId">新创建的帖子 ID。</param>
        /// <returns></returns>
        public static PostCreationResult Success(int postId)
        {
            return new PostCreationResult
            {
                Succeed = true,
                PostId = postId
            };
        }

        /// <summary>
        /// 创建表示操作失败的 <see cref="PostCreationResult"/> 对象。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <returns></returns>
        public static PostCreationResult Failure(string message)
        {
            return new PostCreationResult
            {
                Succeed = false,
                Message = message
            };
        }
    }
}
