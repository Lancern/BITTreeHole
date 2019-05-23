using System;
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

        /// <summary>
        /// 创建一个新的 <see cref="CommentContentEntity"/> 实例对象。
        /// </summary>
        /// <param name="text">评论的文本内容</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/>为null</exception>
        public static CommentContentEntity Create(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return new CommentContentEntity
            {
                Id = ObjectId.GenerateNewId(),
                Text = text
            };
        }
    }
}
