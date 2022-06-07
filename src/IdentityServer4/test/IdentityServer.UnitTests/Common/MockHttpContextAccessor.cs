// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.UnitTests.Common
{
    internal class MockHttpContextAccessor : IHttpContextAccessor
    {
        private HttpContext _context = new DefaultHttpContext();
        public MockAuthenticationService AuthenticationService { get; set; } = new MockAuthenticationService();

        public MockAuthenticationSchemeProvider Schemes { get; set; } = new MockAuthenticationSchemeProvider();

        public MockHttpContextAccessor(
            IdentityServerOptions options = null,
            IUserSession userSession = null,
            IMessageStore<LogoutNotificationContext> endSessionStore = null)
        {
            options = options ?? TestIdentityServerOptions.Create();

            var services = new ServiceCollection();
            services.AddSingleton(options);

            services.AddSingleton<IAuthenticationSchemeProvider>(Schemes);
            services.AddSingleton<IAuthenticationService>(AuthenticationService);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = Schemes.Default;
            });

            if (userSession == null)
            {
                services.AddScoped<IUserSession, DefaultUserSession>();
            }
            else
            {
                services.AddSingleton(userSession);
            }

            if (endSessionStore == null)
            {
                services.AddTransient<IMessageStore<LogoutNotificationContext>, ProtectedDataMessageStore<LogoutNotificationContext>>();
            }
            else
            {
                services.AddSingleton(endSessionStore);
            }

            _context.RequestServices = services.BuildServiceProvider();
        }

        public HttpContext HttpContext
        {
            get
            {
                return _context;
            }

            set
            {
                _context = value;
            }
        }
    }
}
