// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Caching decorator for IResourceStore
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class CachingResourceStore<T> : IResourceStore
        where T : IResourceStore
    {
        private const string AllKey = "__all__";

        private readonly IdentityServerOptions _options;
        
        private readonly ICache<IEnumerable<IdentityResource>> _identityCache;
        private readonly ICache<IEnumerable<ApiResource>> _apiByScopeCache;
        private readonly ICache<IEnumerable<ApiScope>> _apiScopeCache;
        private readonly ICache<IEnumerable<ApiResource>> _apiResourceCache;
        private readonly ICache<Resources> _allCache;
        
        private readonly IResourceStore _inner;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingResourceStore{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="identityCache">The identity cache.</param>
        /// <param name="apiByScopeCache">The API by scope cache.</param>
        /// <param name="apisCache">The API cache.</param>
        /// <param name="scopeCache"></param>
        /// <param name="allCache">All cache.</param>
        /// <param name="logger">The logger.</param>
        public CachingResourceStore(IdentityServerOptions options, T inner, 
            ICache<IEnumerable<IdentityResource>> identityCache, 
            ICache<IEnumerable<ApiResource>> apiByScopeCache,
            ICache<IEnumerable<ApiResource>> apisCache,
            ICache<IEnumerable<ApiScope>> scopeCache,
            ICache<Resources> allCache,
            ILogger<CachingResourceStore<T>> logger)
        {
            _options = options;
            _inner = inner;
            _identityCache = identityCache;
            _apiByScopeCache = apiByScopeCache;
            _apiResourceCache = apisCache;
            _apiScopeCache = scopeCache;
            _allCache = allCache;
            _logger = logger;
        }

        private string GetKey(IEnumerable<string> names)
        {
            if (names == null || !names.Any()) return string.Empty;
            return names.OrderBy(x => x).Aggregate((x, y) => x + "," + y);
        }

        /// <inheritdoc/>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var key = AllKey;

            var all = await _allCache.GetAsync(key,
                _options.Caching.ResourceStoreExpiration,
                async () => await _inner.GetAllResourcesAsync(),
                _logger);

            return all;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var key = GetKey(apiResourceNames);

            var apis = await _apiResourceCache.GetAsync(key,
                _options.Caching.ResourceStoreExpiration,
                async () => await _inner.FindApiResourcesByNameAsync(apiResourceNames),
                _logger);

            return apis;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> names)
        {
            var key = GetKey(names);

            var identities = await _identityCache.GetAsync(key,
                _options.Caching.ResourceStoreExpiration,
                async () => await _inner.FindIdentityResourcesByScopeNameAsync(names),
                _logger);

            return identities;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> names)
        {
            var key = GetKey(names);

            var apis = await _apiByScopeCache.GetAsync(key,
                _options.Caching.ResourceStoreExpiration,
                async () => await _inner.FindApiResourcesByScopeNameAsync(names),
                _logger);

            return apis;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var key = GetKey(scopeNames);

            var apis = await _apiScopeCache.GetAsync(key,
                _options.Caching.ResourceStoreExpiration,
                async () => await _inner.FindApiScopesByNameAsync(scopeNames),
                _logger);

            return apis;
        }
    }
}
