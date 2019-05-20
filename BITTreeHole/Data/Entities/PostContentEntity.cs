using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示帖子内容实体对象。
    /// </summary>
    public sealed class PostContentEntity
    {
        /// <summary>
        /// 获取或设置帖子内容实体对象 ID。
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// 获取或设置帖子正文。
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// 获取或设置帖子所附带的图片 ID。
        /// </summary>
        public ObjectId[] ImageIds { get; set; }

        /// <summary>
        /// 创建新的 <see cref="PostContentEntity"/> 对象。该方法会自动为返回的对象创建 <see cref="ObjectId"/> 键。
        /// </summary>
        /// <returns>创建的 <see cref="PostContentEntity"/> 对象。</returns>
        public static PostContentEntity Create()
        {
            return new PostContentEntity
            {
                Id = ObjectId.GenerateNewId(),
                Text = string.Empty,
                ImageIds = new ObjectId[0]
            };
        }
    }
}
