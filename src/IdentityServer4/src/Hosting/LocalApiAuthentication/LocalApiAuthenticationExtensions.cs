// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4;
using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for registering the local access token authentication handler
    /// </summary>
    public static class LocalApiAuthenticationExtensions
    {
        /// <summary>
        /// Adds support for local APIs
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="transformationFunc">Function to transform the resulting principal</param>
        /// <returns></returns>
        public static IServiceCollection AddLocalApiAuthentication(this IServiceCollection services, Func<ClaimsPrincipal, Task<ClaimsPrincipal>> transformationFunc = null)
        {
            services.AddAuthentication()
                .AddLocalApi(options =>
                {
                    options.ExpectedScope = IdentityServerConstants.LocalApi.ScopeName;

                    if (transformationFunc != null)
                    {
                        options.Events = new LocalApiAuthenticationEvents
                        {
                            OnClaimsTransformation = async e =>
                            {
                                e.Principal = await transformationFunc(e.Principal);
                            }
                        };
                    }
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            return services;
        }

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder)
            => builder.AddLocalApi(IdentityServerConstants.LocalApi.AuthenticationScheme, _ => { });

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, Action<LocalApiAuthenticationOptions> configureOptions)
            => builder.AddLocalApi(IdentityServerConstants.LocalApi.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, string authenticationScheme, Action<LocalApiAuthenticationOptions> configureOptions)
            => builder.AddLocalApi(authenticationScheme, displayName: null, configureOptions: configureOptions);

        /// <summary>
        /// Registers the authentication handler for local APIs.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name of this scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLocalApi(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<LocalApiAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<LocalApiAuthenticationOptions, LocalApiAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
