// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Configuration.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Hosting.FederatedSignOut
{
    // this intercepts IAuthenticationRequestHandler authentication handlers
    // to detect when they are handling federated signout. when they are invoked,
    // call signout on the default authentication scheme, and return 200 then 
    // we assume they are handling the federated signout in an iframe. 
    // based on this assumption, we then render our federated signout iframes 
    // to any current clients.
    internal class FederatedSignoutAuthenticationHandlerProvider : IAuthenticationHandlerProvider
    {
        private readonly IAuthenticationHandlerProvider _provider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FederatedSignoutAuthenticationHandlerProvider(
            Decorator<IAuthenticationHandlerProvider> decorator, 
            IHttpContextAccessor httpContextAccessor)
        {
            _provider = decorator.Instance;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IAuthenticationHandler> GetHandlerAsync(HttpContext context, string authenticationScheme)
        {
            var handler = await _provider.GetHandlerAsync(context, authenticationScheme);
            if (handler is IAuthenticationRequestHandler requestHandler)
            {
                if (requestHandler is IAuthenticationSignInHandler signinHandler)
                {
                    return new AuthenticationRequestSignInHandlerWrapper(signinHandler, _httpContextAccessor);
                }

                if (requestHandler is IAuthenticationSignOutHandler signoutHandler)
                {
                    return new AuthenticationRequestSignOutHandlerWrapper(signoutHandler, _httpContextAccessor);
                }

                return new AuthenticationRequestHandlerWrapper(requestHandler, _httpContextAccessor);
            }

            return handler;
        }
    }
}
