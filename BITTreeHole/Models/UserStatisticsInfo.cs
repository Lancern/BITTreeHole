using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装用户个人统计信息。
    /// </summary>
    public sealed class UserStatisticsInfo
    {
        /// <summary>
        /// 初始化 <see cref="UserStatisticsInfo"/> 类的新实例。
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="numberOfPosts">用户发表的帖子数量</param>
        /// <param name="numberOfReceivedVotes">用户收到的点赞数量</param>
        public UserStatisticsInfo(int userId, int numberOfPosts, int numberOfReceivedVotes)
        {
            UserId = userId;
            NumberOfPosts = numberOfPosts;
            NumberOfReceivedVotes = numberOfReceivedVotes;
        }
        
        /// <summary>
        /// 获取用户 ID
        /// </summary>
        [JsonProperty("id")]
        public int UserId { get; }
        
        /// <summary>
        /// 获取用户发出的帖子数量
        /// </summary>
        [JsonProperty("numberOfPosts")]
        public int NumberOfPosts { get; }
        
        /// <summary>
        /// 获取用户收到的点赞数量
        /// </summary>
        [JsonProperty("numberOfReceivedVotes")]
        public int NumberOfReceivedVotes { get; }
    }
}
