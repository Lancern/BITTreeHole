using System;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Contexts;
using BITTreeHole.Data.Entities;
using MySql.Data.MySqlClient;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 提供 <see cref="IDataFacade"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultDataFacade : IDataFacade
    {
        private readonly MysqlDbContext _mysqlDbContext;

        /// <summary>
        /// 初始化 <see cref="DefaultDataFacade"/> 类的新实例。
        /// </summary>
        /// <param name="mysqlDbContext">MySQL数据上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="mysqlDbContext"/>为null。</exception>
        public DefaultDataFacade(MysqlDbContext mysqlDbContext)
        {
            _mysqlDbContext = mysqlDbContext ?? throw new ArgumentNullException(nameof(mysqlDbContext));
        }
        
        /// <inheritdoc />
        public IQueryable<UserEntity> Users { get; }

        /// <inheritdoc />
        public void AddUser(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            _mysqlDbContext.Users.AddAsync(user);
        }

        /// <inheritdoc />
        public void RemoveUser(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            _mysqlDbContext.Users.Remove(user);
        }

        /// <inheritdoc />
        public async Task CommitChanges()
        {
            try
            {
                await _mysqlDbContext.SaveChangesAsync();
            }
            catch (MySqlException ex)
            {
                throw new DataFacadeException("MySQL数据源抛出了未经处理的异常。", ex);
            }
        }
    }
}
