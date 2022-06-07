// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Linq;
using IdentityServer4.Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Configuration
{
    internal class ConfigureOpenIdConnectOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        private string[] _schemes;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfigureOpenIdConnectOptions(string[] schemes, IHttpContextAccessor httpContextAccessor)
        {
            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void PostConfigure(string name, OpenIdConnectOptions options)
        {
            // no schemes means configure them all
            if (_schemes.Length == 0 || _schemes.Contains(name))
            {
                options.StateDataFormat = new DistributedCacheStateDataFormatter(_httpContextAccessor, name);
            }
        }
    }
}
