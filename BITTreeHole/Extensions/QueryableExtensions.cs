using System;
using System.Linq;

namespace BITTreeHole.Extensions
{
    /// <summary>
    /// 为 <see cref="IQueryable{T}"/> 提供扩展方法。
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 应用分页参数到指定的 <see cref="IQueryable{T}"/> 对象上。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="page">页面编号。页面编号从 0 开始。</param>
        /// <param name="itemsPerPage">每一页上的数据项目数量。</param>
        /// <typeparam name="T">数据项类型。</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="page"/>小于零
        ///     或
        ///     <paramref name="itemsPerPage"/>小于等于零
        /// </exception>
        /// <exception cref="OverflowException">
        ///     <paramref name="page"/>与<paramref name="itemsPerPage"/>的乘积造成了整数溢出
        /// </exception>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int itemsPerPage)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (itemsPerPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(itemsPerPage));

            var skippedCount = checked(page * itemsPerPage);
            return source.Skip(skippedCount).Take(itemsPerPage);
        }
    }
}
