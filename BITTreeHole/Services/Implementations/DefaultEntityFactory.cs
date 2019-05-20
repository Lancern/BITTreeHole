using System;
using BITTreeHole.Data.Entities;
using BITTreeHole.Models;

namespace BITTreeHole.Services.Implementations
{
    /// <summary>
    /// 为 <see cref="IEntityFactory"/> 提供默认实现。
    /// </summary>
    internal sealed class DefaultEntityFactory : IEntityFactory
    {
        /// <inheritdoc />
        public (PostEntity IndexEntity, PostContentEntity ContentEntity) CreatePostEntities(
            int authorId, PostInfo infoModel)
        {
            if (infoModel == null)
                throw new ArgumentNullException(nameof(infoModel));

            var contentEntity = PostContentEntity.Create();
            contentEntity.Text = infoModel.Text;

            var indexEntity = PostEntity.Create(authorId, infoModel.RegionId, infoModel.Title,
                                                contentEntity.Id.ToByteArray());
            return (indexEntity, contentEntity);
        }
    }
}
