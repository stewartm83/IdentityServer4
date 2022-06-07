// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Extension methods for signin/out using the IdentityServer authentication scheme.
    /// </summary>
    public static class AuthenticationManagerExtensions
    {
        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="context">The manager.</param>
        /// <param name="user">The IdentityServer user.</param>
        /// <returns></returns>
        public static async Task SignInAsync(this HttpContext context, IdentityServerUser user)
        {
            await context.SignInAsync(await context.GetCookieAuthenticationSchemeAsync(), user.CreatePrincipal());
        }

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="context">The manager.</param>
        /// <param name="user">The IdentityServer user.</param>
        /// <param name="properties">The authentication properties.</param>
        /// <returns></returns>
        public static async Task SignInAsync(this HttpContext context, IdentityServerUser user, AuthenticationProperties properties)
        {
            await context.SignInAsync(await context.GetCookieAuthenticationSchemeAsync(), user.CreatePrincipal(), properties);
        }

        internal static ISystemClock GetClock(this HttpContext context)
        {
            return context.RequestServices.GetRequiredService<ISystemClock>();
        }

        internal static async Task<string> GetCookieAuthenticationSchemeAsync(this HttpContext context)
        {
            var options = context.RequestServices.GetRequiredService<IdentityServerOptions>();
            if (options.Authentication.CookieAuthenticationScheme != null)
            {
                return options.Authentication.CookieAuthenticationScheme;
            }

            var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme = await schemes.GetDefaultAuthenticateSchemeAsync();
            if (scheme == null)
            {
                throw new InvalidOperationException("No DefaultAuthenticateScheme found or no CookieAuthenticationScheme configured on IdentityServerOptions.");
            }

            return scheme.Name;
        }
    }
}
