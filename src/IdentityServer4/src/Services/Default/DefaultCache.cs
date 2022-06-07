// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace IdentityServer4.Services
{
    /// <summary>
    /// IMemoryCache-based implementation of the cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IdentityServer4.Services.ICache{T}" />
    public class DefaultCache<T> : ICache<T>
        where T : class
    {
        private const string KeySeparator = ":";

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCache{T}"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public DefaultCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        private string GetKey(string key)
        {
            return typeof(T).FullName + KeySeparator + key;
        }

        /// <summary>
        /// Gets the cached data based upon a key index.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The cached item, or <c>null</c> if no item matches the key.
        /// </returns>
        public Task<T> GetAsync(string key)
        {
            key = GetKey(key);
            var item = _cache.Get<T>(key);
            return Task.FromResult(item);
        }

        /// <summary>
        /// Caches the data based upon a key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        /// <returns></returns>
        public Task SetAsync(string key, T item, TimeSpan expiration)
        {
            key = GetKey(key);
            _cache.Set(key, item, expiration);
            return Task.CompletedTask;
        }
    }
}
