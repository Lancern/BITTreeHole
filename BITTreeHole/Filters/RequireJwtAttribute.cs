using System;
using System.Linq;
using BITTreeHole.Data;
using BITTreeHole.Extensions;
using BITTreeHole.Models;
using BITTreeHole.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Filters
{
    /// <summary>
    /// 访问被修饰的控制器或方法需要当前的 HTTP 请求中带有有效的 Jwt 头部字段。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireJwtAttribute : Attribute, IFilterFactory
    {
        /// <summary>
        /// 为 <see cref="RequireJwtAttribute"/> 提供实际的过滤器实现。
        /// </summary>
        private sealed class RequireJwtImplAttribute : IAuthorizationFilter
        {
            private readonly IJwtService _jwt;
            private readonly IDataFacade _dataFacade;
            private readonly bool _requireAdmin;

            /// <summary>
            /// 初始化 <see cref="RequireJwtImplAttribute"/> 类的新实例。
            /// </summary>
            /// <param name="jwtService">由 DI 解析的 JWT 服务实例。</param>
            /// <param name="dataFacade">数据层外观。</param>
            /// <param name="requireAdmin">是否要求用户为管理员。</param>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="jwtService"/>为null
            /// </exception>
            public RequireJwtImplAttribute(IJwtService jwtService, IDataFacade dataFacade, bool requireAdmin)
            {
                _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
                _dataFacade = dataFacade ?? throw new ArgumentNullException(nameof(dataFacade));
                _requireAdmin = requireAdmin;
            }

            /// <summary>
            /// 检查给定的 <see cref="AuthenticationToken"/> 所标识的用户是否具有管理员权限。
            /// </summary>
            /// <param name="token"></param>
            /// <returns>用户是否具有管理员权限。</returns>
            private bool AuthorizeAdmin(AuthenticationToken token)
            {
                var userEntity = _dataFacade.Users
                                            .AsNoTracking()
                                            .Include(entity => entity.IsAdmin)
                                            .FirstOrDefault(entity => entity.Id == token.UserId);
                return userEntity != null && userEntity.IsAdmin;
            }
            
            /// <summary>
            /// 检查给定的请求上下文中是否包含有效的 Jwt 信息。
            /// </summary>
            /// <param name="context"></param>
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!context.HttpContext.Request.Headers.TryGetValue("Jwt", out var jwtHeader))
                {
                    // 请求头部中没有 Jwt 头部。返回 403 Forbidden。
                    context.Result = new StatusCodeResult(403);
                    return;
                }
                
                AuthenticationToken authToken;
                try
                {
                    authToken = _jwt.Decode<AuthenticationToken>(jwtHeader);
                }
                catch
                {
                    // 无效的 JWT 头部。
                    context.Result = new StatusCodeResult(403);
                    return;
                }

                if (_requireAdmin && !AuthorizeAdmin(authToken))
                {
                    // 用户不具备管理员权限。
                    context.Result = new StatusCodeResult(403);
                    return;
                }
                
                // 将 authToken 绑定到当前的 HTTP 会话上
                context.HttpContext.SetAuthenticationToken(authToken);
            }
        }

        /// <summary>
        /// 初始化 <see cref="RequireJwtAttribute"/> 类的新实例。
        /// </summary>
        public RequireJwtAttribute()
        {
            RequireAdmin = false;
        }
        
        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var jwtService = serviceProvider.GetService<IJwtService>();
            var dataFacade = serviceProvider.GetService<IDataFacade>();
            return new RequireJwtImplAttribute(jwtService, dataFacade, RequireAdmin);
        }

        /// <inheritdoc />
        /// <remarks>
        /// 由于选项 <see cref="RequireAdmin"/> 的存在，不能重用已经创建的过滤器实例。
        /// </remarks>
        public bool IsReusable => false;
        
        /// <summary>
        /// 是否要求用户为管理员。
        /// </summary>
        public bool RequireAdmin { get; set; }
    }
}
