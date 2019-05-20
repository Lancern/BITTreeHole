using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Entities;
using BITTreeHole.Extensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 为 <see cref="IDataFacade"/> 提供扩展方法。
    /// </summary>
    public static class DataFacadeExtensions
    {
        /// <summary>
        /// 当给定的用户不存在时，向数据源添加用户。返回用户实体对象。
        /// </summary>
        /// <param name="dataFacade">数据源外观。</param>
        /// <param name="wechatId">用户的微信 ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/> 为 null
        ///     或
        ///     <paramref name="wechatId"/> 为 null
        /// </exception>
        public static async Task<UserEntity> AddOrFindUserByWechatId(this IDataFacade dataFacade, string wechatId)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (wechatId == null)
                throw new ArgumentNullException(nameof(wechatId));

            var userEntity = new UserEntity
            {
                WechatId = wechatId
            };

            try
            {
                dataFacade.AddUser(userEntity);
                await dataFacade.CommitChanges();
                return userEntity;
            }
            catch (DataFacadeException)
            {
            }
            
            // 用户已经存在于数据源中
            return await dataFacade.Users
                                   .Where(u => u.WechatId == wechatId)
                                   .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 向数据源中添加帖子。
        /// </summary>
        /// <param name="dataFacade">数据源的外观。</param>
        /// <param name="indexEntity">帖子索引实体对象。</param>
        /// <param name="contentEntity">帖子内容对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/>为null
        ///     或
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        /// </exception>
        /// <exception cref="DataFacadeException">数据源抛出了未经处理的异常</exception>
        public static async Task<int> AddPost(this IDataFacade dataFacade,
                                              PostEntity indexEntity, PostContentEntity contentEntity)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));
            
            // 将内容实体对象插入到数据源中。
            await dataFacade.AddPostContentEntity(contentEntity);
            
            // 将帖子索引实体对象插入到数据源中。
            dataFacade.AddPostIndexEntity(indexEntity);
            await dataFacade.CommitChanges();

            return indexEntity.Id;
        }

        /// <summary>
        /// 查询帖子信息。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="region">帖子所属板块 ID。</param>
        /// <param name="page">分页参数，页面编号。页面编号从 0 开始。</param>
        /// <param name="itemsPerPage">分页参数，每一页上的帖子数量。</param>
        /// <returns>帖子信息实体对象与帖子正文实体对象二元组集合</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="page"/>小于零
        ///     或
        ///     <paramref name="itemsPerPage"/>小于等于零
        /// </exception>
        public static async Task<IEnumerable<(PostEntity IndexEntity, PostContentEntity ContentEntity)>>
            FindPosts(this IDataFacade dataFacade, int region, int page, int itemsPerPage)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (itemsPerPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(itemsPerPage));
            
            var indexEntities = await dataFacade.Posts
                                                .AsNoTracking()
                                                // 下面的语句中务必使用 e.IsRemoved == false 以正确引导 EFCore 解析查询
                                                .Where(e => e.PostRegionId == region && e.IsRemoved == false)
                                                .OrderByDescending(e => e.UpdateTime)
                                                .Paginate(page, itemsPerPage)
                                                .ToListAsync();
            var contentEntities =
                await dataFacade.FindPostContentEntities(indexEntities.Select(e => new ObjectId(e.ContentId)));
            var contentDict = contentEntities.ToDictionary(e => e.Id);

            return indexEntities.Select(indexEntity =>
            {
                if (contentDict.TryGetValue(new ObjectId(indexEntity.ContentId), out var contentEntity))
                    return (indexEntity, contentEntity);
                return (indexEntity, null);
            });
        }
    }
}
