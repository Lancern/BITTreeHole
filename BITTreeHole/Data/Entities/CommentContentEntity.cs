using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示评论内容实体对象。
    /// </summary>
    public sealed class CommentContentEntity
    {
        /// <summary>
        /// 获取或设置该实体对象的 ID。
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// 获取或设置评论内容。
        /// </summary>
        public string Text { get; set; }
    }
}
