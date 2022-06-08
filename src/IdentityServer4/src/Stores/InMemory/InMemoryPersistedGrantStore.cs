// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// In-memory persisted grant store
    /// </summary>
    public class InMemoryPersistedGrantStore : IPersistedGrantStore
    {
        private readonly ConcurrentDictionary<string, PersistedGrant> _repository = new ConcurrentDictionary<string, PersistedGrant>();

        /// <inheritdoc/>
        public Task StoreAsync(PersistedGrant grant)
        {
            _repository[grant.Key] = grant;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<PersistedGrant> GetAsync(string key)
        {
            if (_repository.TryGetValue(key, out PersistedGrant token))
            {
                return Task.FromResult(token);
            }

            return Task.FromResult<PersistedGrant>(null);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();
            
            var items = Filter(filter);
            
            return Task.FromResult(items);
        }

        /// <inheritdoc/>
        public Task RemoveAsync(string key)
        {
            _repository.TryRemove(key, out _);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var items = Filter(filter);
            
            foreach (var item in items)
            {
                _repository.TryRemove(item.Key, out _);
            }

            return Task.CompletedTask;
        }

        private IEnumerable<PersistedGrant> Filter(PersistedGrantFilter filter)
        {
            var query =
                from item in _repository
                select item.Value;

            if (!String.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query.Where(x => x.SessionId == filter.SessionId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (!String.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            var items = query.ToArray().AsEnumerable();
            return items;
        }
    }
}
