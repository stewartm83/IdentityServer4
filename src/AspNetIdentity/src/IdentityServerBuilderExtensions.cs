﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Linq;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add ASP.NET Identity support to IdentityServer.
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Configures IdentityServer to use the ASP.NET Identity implementations 
        /// of IUserClaimsPrincipalFactory, IResourceOwnerPasswordValidator, and IProfileService.
        /// Also configures some of ASP.NET Identity's options for use with IdentityServer (such as claim types to use
        /// and authentication cookie settings).
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class
        {
            builder.Services.AddTransientDecorator<IUserClaimsPrincipalFactory<TUser>, UserClaimsFactory<TUser>>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // we need to disable to allow iframe for authorize requests
                options.Cookie.SameSite = AspNetCore.Http.SameSiteMode.None;
            });

            builder.Services.ConfigureExternalCookie(options =>
            {
                options.Cookie.IsEssential = true;
                // https://github.com/stewartm83/IdentityServer4/issues/2595
                options.Cookie.SameSite = AspNetCore.Http.SameSiteMode.None;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options =>
            {
                options.Cookie.IsEssential = true;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options =>
            {
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(options =>
            {
                if (options.DefaultAuthenticateScheme == null &&
                    options.DefaultScheme == IdentityServerConstants.DefaultCookieAuthenticationScheme)
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                }
            });

            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
            builder.AddProfileService<ProfileService<TUser>>();

            return builder;
        }

        internal static void AddTransientDecorator<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddDecorator<TService>();
            services.AddTransient<TService, TImplementation>();
        }

        internal static void AddDecorator<TService>(this IServiceCollection services)
        {
            var registration = services.LastOrDefault(x => x.ServiceType == typeof(TService));
            if (registration == null)
            {
                throw new InvalidOperationException("Service type: " + typeof(TService).Name + " not registered.");
            }
            if (services.Any(x => x.ServiceType == typeof(Decorator<TService>)))
            {
                throw new InvalidOperationException("Decorator already registered for type: " + typeof(TService).Name + ".");
            }

            services.Remove(registration);

            if (registration.ImplementationInstance != null)
            {
                var type = registration.ImplementationInstance.GetType();
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), type);
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(type, registration.ImplementationInstance));
            }
            else if (registration.ImplementationFactory != null)
            {
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), provider =>
                {
                    return new DisposableDecorator<TService>((TService)registration.ImplementationFactory(provider));
                }, registration.Lifetime));
            }
            else
            {
                var type = registration.ImplementationType;
                var innerType = typeof(Decorator<,>).MakeGenericType(typeof(TService), registration.ImplementationType);
                services.Add(new ServiceDescriptor(typeof(Decorator<TService>), innerType, ServiceLifetime.Transient));
                services.Add(new ServiceDescriptor(type, type, registration.Lifetime));
            }
        }
    }
}
