using System;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Entities;
using Microsoft.EntityFrameworkCore;

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
    }
}
