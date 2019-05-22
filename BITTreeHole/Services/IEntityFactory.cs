using System;
using BITTreeHole.Data.Entities;
using BITTreeHole.Models;
using BITTreeHole.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 为数据实体对象提供工厂方法的依赖注入抽象。
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// 创建帖子实体对象。
        /// </summary>
        /// <param name="authorId">发帖用户 ID。</param>
        /// <param name="creationInfoModel">帖子信息模型。</param>
        /// <returns>帖子索引实体对象与帖子内容实体对象。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        (PostEntity IndexEntity, PostContentEntity ContentEntity) CreatePostEntities(int authorId, PostCreationInfo creationInfoModel);
    }

    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="IEntityFactory"/> 提供依赖注入过程。
        /// </summary>
        public static class EntityFactoryExtensions
        {
            /// <summary>
            /// 将默认的 <see cref="IEntityFactory"/> 实现注入到依赖服务集中。
            /// </summary>
            /// <param name="services">依赖服务集。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="services"/>为null
            /// </exception>
            public static IServiceCollection AddDefaultEntityFactory(this IServiceCollection services)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                return services.AddSingleton<IEntityFactory, DefaultEntityFactory>();
            }
        }
    }
}
