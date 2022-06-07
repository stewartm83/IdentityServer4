// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors.Infrastructure;
using IdentityServer4.Configuration;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Extensions;

namespace IdentityServer4.Hosting
{
    internal class CorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly ILogger _logger;
        private readonly ICorsPolicyProvider _inner;
        private readonly IdentityServerOptions _options;
        private readonly IHttpContextAccessor _httpContext;

        public CorsPolicyProvider(
            ILogger<CorsPolicyProvider> logger,
            Decorator<ICorsPolicyProvider> inner,
            IdentityServerOptions options,
            IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _inner = inner.Instance;
            _options = options;
            _httpContext = httpContext;
        }

        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            if (_options.Cors.CorsPolicyName == policyName)
            {
                return ProcessAsync(context);
            }
            else
            {
                return _inner.GetPolicyAsync(context, policyName);
            }
        }

        private async Task<CorsPolicy> ProcessAsync(HttpContext context)
        {
            var origin = context.Request.GetCorsOrigin();
            if (origin != null)
            {
                var path = context.Request.Path;
                if (IsPathAllowed(path))
                {
                    _logger.LogDebug("CORS request made for path: {path} from origin: {origin}", path, origin);

                    // manually resolving this from DI because this: 
                    // https://github.com/aspnet/CORS/issues/105
                    var corsPolicyService = _httpContext.HttpContext.RequestServices.GetRequiredService<ICorsPolicyService>();

                    if (await corsPolicyService.IsOriginAllowedAsync(origin))
                    {
                        _logger.LogDebug("CorsPolicyService allowed origin: {origin}", origin);
                        return Allow(origin);
                    }
                    else
                    {
                        _logger.LogWarning("CorsPolicyService did not allow origin: {origin}", origin);
                    }
                }
                else
                {
                    _logger.LogDebug("CORS request made for path: {path} from origin: {origin} but was ignored because path was not for an allowed IdentityServer CORS endpoint", path, origin);
                }
            }

            return null;
        }

        private CorsPolicy Allow(string origin)
        {
            var policyBuilder = new CorsPolicyBuilder()
                .WithOrigins(origin)
                .AllowAnyHeader()
                .AllowAnyMethod();

            if (_options.Cors.PreflightCacheDuration.HasValue)
            {
                policyBuilder.SetPreflightMaxAge(_options.Cors.PreflightCacheDuration.Value);
            }

            return policyBuilder.Build();
        }

        private bool IsPathAllowed(PathString path)
        {
            return _options.Cors.CorsPaths.Any(x => path == x);
        }
    }
}
