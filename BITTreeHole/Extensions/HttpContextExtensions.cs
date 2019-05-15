using System;
using BITTreeHole.Models;
using Microsoft.AspNetCore.Http;

namespace BITTreeHole.Extensions
{
    /// <summary>
    /// 为 <see cref="HttpContext"/> 提供扩展方法。
    /// </summary>
    public static class HttpContextExtensions
    {
        private const string AuthenticationTokenKey = "AppAuthToken";
        
        /// <summary>
        /// 将给定的 <see cref="AuthenticationToken"/> 绑定到给定的 <see cref="HttpContext"/> 对象上。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context"/>为null
        ///     或
        ///     <paramref name="token"/>为null
        /// </exception>
        public static void SetAuthenticationToken(this HttpContext context,
                                                  AuthenticationToken token)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            context.Items[AuthenticationTokenKey] = token;
        }

        /// <summary>
        /// 从给定的 <see cref="HttpContext"/> 中抽取 <see cref="AuthenticationToken"/> 对象。
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// 从给定的 <see cref="HttpContext"/> 中抽取的 <see cref="AuthenticationToken"/> 对象。
        /// 若给定的 <see cref="HttpContext"/> 中没有 <see cref="AuthenticationToken"/> 对象，返回 null。
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context"/>为null
        /// </exception>
        public static AuthenticationToken GetAuthenticationToken(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Items.TryGetValue(AuthenticationTokenKey, out var authToken))
                return null;
            return authToken as AuthenticationToken;
        }
    }
}
