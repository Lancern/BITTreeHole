using System;
using BITTreeHole.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace BITTreeHole.Data.Contexts
{
    /// <summary>
    /// 为 MongoDB 提供数据库上下文。
    /// </summary>
    public sealed class MongoDbContext
    {
        private const string DatabaseName = "BITTreeHoleDB";
        private const string PostContentCollectionName = "Posts";
        private const string CommentContentCollectionName = "Comments";

        private readonly IMongoDatabase _db;
        
        /// <summary>
        /// 初始化 <see cref="MongoDbContext"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">到 MongoDB 实例的连接字符串。</param>
        /// <exception cref="ArgumentNullException"><paramref name="connectionString"/>为null</exception>
        public MongoDbContext(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(DatabaseName);
        }

        /// <summary>
        /// 获取包含帖子正文的 <see cref="IMongoCollection{TDocument}"/> 对象。
        /// </summary>
        public IMongoCollection<PostContentEntity> PostContents =>
            _db.GetCollection<PostContentEntity>(PostContentCollectionName);

        /// <summary>
        /// 获取包含评论正文的 <see cref="IMongoCollection{TDocument}"/> 对象。
        /// </summary>
        public IMongoCollection<CommentContentEntity> CommentContents =>
            _db.GetCollection<CommentContentEntity>(CommentContentCollectionName);
        
        /// <summary>
        /// 获取包含帖子中图片的 <see cref="IGridFSBucket"/> 对象。
        /// </summary>
        public IGridFSBucket ImageBucket =>
            new GridFSBucket(_db);
    }

    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="MongoDbContext"/> 提供依赖注入逻辑。
        /// </summary>
        public static class MongoDbContextExtensions
        {
            /// <summary>
            /// 将 <see cref="MongoDbContext"/> 依赖项添加到给定的依赖服务项集合中。
            /// </summary>
            /// <param name="services"></param>
            /// <param name="connectionString">到 MongoDB 实例的连接字符串。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="services"/>为null
            ///     或
            ///     <paramref name="connectionString"/>为null
            /// </exception>
            public static IServiceCollection AddMongoDbContext(this IServiceCollection services,
                                                               string connectionString)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                if (connectionString == null)
                    throw new ArgumentNullException(nameof(connectionString));

                return services.AddScoped(_ => new MongoDbContext(connectionString));
            }
        }
    }
}
