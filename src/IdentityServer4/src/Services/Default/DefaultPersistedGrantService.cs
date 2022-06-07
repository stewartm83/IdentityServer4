// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Default persisted grant service
    /// </summary>
    public class DefaultPersistedGrantService : IPersistedGrantService
    {
        private readonly ILogger _logger;
        private readonly IPersistedGrantStore _store;
        private readonly IPersistentGrantSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPersistedGrantService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="logger">The logger.</param>
        public DefaultPersistedGrantService(IPersistedGrantStore store, 
            IPersistentGrantSerializer serializer,
            ILogger<DefaultPersistedGrantService> logger)
        {
            _store = store;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Grant>> GetAllGrantsAsync(string subjectId)
        {
            if (String.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));
            
            var grants = (await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = subjectId })).ToArray();

            try
            {
                var consents = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.UserConsent)
                    .Select(x => _serializer.Deserialize<Consent>(x.Data))
                    .Select(x => new Grant 
                    {
                        ClientId = x.ClientId,
                        SubjectId = subjectId,
                        Scopes = x.Scopes,
                        CreationTime = x.CreationTime,
                        Expiration = x.Expiration
                    });

                var codes = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.AuthorizationCode)
                    .Select(x => _serializer.Deserialize<AuthorizationCode>(x.Data))
                    .Select(x => new Grant
                    {
                        ClientId = x.ClientId,
                        SubjectId = subjectId,
                        Description = x.Description,
                        Scopes = x.RequestedScopes,
                        CreationTime = x.CreationTime,
                        Expiration = x.CreationTime.AddSeconds(x.Lifetime)
                    });

                var refresh = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.RefreshToken)
                    .Select(x => _serializer.Deserialize<RefreshToken>(x.Data))
                    .Select(x => new Grant
                    {
                        ClientId = x.ClientId,
                        SubjectId = subjectId,
                        Description = x.Description,
                        Scopes = x.Scopes,
                        CreationTime = x.CreationTime,
                        Expiration = x.CreationTime.AddSeconds(x.Lifetime)
                    });

                var access = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.ReferenceToken)
                    .Select(x => _serializer.Deserialize<Token>(x.Data))
                    .Select(x => new Grant
                    {
                        ClientId = x.ClientId,
                        SubjectId = subjectId,
                        Description = x.Description,
                        Scopes = x.Scopes,
                        CreationTime = x.CreationTime,
                        Expiration = x.CreationTime.AddSeconds(x.Lifetime)
                    });

                consents = Join(consents, codes);
                consents = Join(consents, refresh);
                consents = Join(consents, access);

                return consents.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing results from grant store.");
            }

            return Enumerable.Empty<Grant>();
        }

        private IEnumerable<Grant> Join(IEnumerable<Grant> first, IEnumerable<Grant> second)
        {
            var list = first.ToList();

            foreach(var other in second)
            {
                var match = list.FirstOrDefault(x => x.ClientId == other.ClientId);
                if (match != null)
                {
                    match.Scopes = match.Scopes.Union(other.Scopes).Distinct();

                    if (match.CreationTime > other.CreationTime)
                    {
                        // show the earlier creation time
                        match.CreationTime = other.CreationTime;
                    }

                    if (match.Expiration == null || other.Expiration == null)
                    {
                        // show that there is no expiration to one of the grants
                        match.Expiration = null;
                    }
                    else if (match.Expiration < other.Expiration)
                    {
                        // show the latest expiration
                        match.Expiration = other.Expiration;
                    }

                    match.Description = match.Description ?? other.Description;
                }
                else
                {
                    list.Add(other);
                }
            }

            return list;
        }

        /// <inheritdoc/>
        public Task RemoveAllGrantsAsync(string subjectId, string clientId = null, string sessionId = null)
        {
            if (String.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));

            return _store.RemoveAllAsync(new PersistedGrantFilter {
                SubjectId = subjectId,
                ClientId = clientId,
                SessionId = sessionId
            });
        }
    }
}
