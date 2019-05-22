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
            int authorId, PostCreationInfo creationInfoModel)
        {
            if (creationInfoModel == null)
                throw new ArgumentNullException(nameof(creationInfoModel));

            var contentEntity = PostContentEntity.Create();
            contentEntity.Text = creationInfoModel.Text;

            var indexEntity = PostEntity.Create(authorId, creationInfoModel.RegionId, creationInfoModel.Title,
                                                contentEntity.Id.ToByteArray());
            return (indexEntity, contentEntity);
        }
    }
}
