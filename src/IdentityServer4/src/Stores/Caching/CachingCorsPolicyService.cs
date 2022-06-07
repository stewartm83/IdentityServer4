// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Caching decorator for ICorsPolicyService
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.ICorsPolicyService" />
    public class CachingCorsPolicyService<T> : ICorsPolicyService
        where T : ICorsPolicyService
    {
        /// <summary>
        /// Class to model entries in CORS origin cache.
        /// </summary>
        public class CorsCacheEntry
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            public CorsCacheEntry(bool allowed)
            {
                Allowed = allowed;
            }

            /// <summary>
            /// Is origin allowed.
            /// </summary>
            public bool Allowed { get; }
        }

        private readonly IdentityServerOptions Options;
        private readonly ICache<CorsCacheEntry> CorsCache;
        private readonly ICorsPolicyService Inner;
        private readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingResourceStore{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="corsCache">The CORS origin cache.</param>
        /// <param name="logger">The logger.</param>
        public CachingCorsPolicyService(IdentityServerOptions options,
            T inner,
            ICache<CorsCacheEntry> corsCache,
            ILogger<CachingCorsPolicyService<T>> logger)
        {
            Options = options;
            Inner = inner;
            CorsCache = corsCache;
            Logger = logger;
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public virtual async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var entry = await CorsCache.GetAsync(origin,
                          Options.Caching.CorsExpiration,
                          async () => new CorsCacheEntry(await Inner.IsOriginAllowedAsync(origin)),
                          Logger);

            return entry.Allowed;
        }
    }
}
