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
