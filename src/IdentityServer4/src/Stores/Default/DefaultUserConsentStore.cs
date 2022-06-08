// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Default user consent store.
    /// </summary>
    public class DefaultUserConsentStore : DefaultGrantStore<Consent>, IUserConsentStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUserConsentStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handleGenerationService">The handle generation service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultUserConsentStore(
            IPersistedGrantStore store, 
            IPersistentGrantSerializer serializer,
            IHandleGenerationService handleGenerationService,
            ILogger<DefaultUserConsentStore> logger) 
            : base(IdentityServerConstants.PersistedGrantTypes.UserConsent, store, serializer, handleGenerationService, logger)
        {
        }

        private string GetConsentKey(string subjectId, string clientId)
        {
            return clientId + "|" + subjectId;
        }

        /// <summary>
        /// Stores the user consent asynchronous.
        /// </summary>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        public Task StoreUserConsentAsync(Consent consent)
        {
            var key = GetConsentKey(consent.SubjectId, consent.ClientId);
            return StoreItemAsync(key, consent, consent.ClientId, consent.SubjectId, null, null, consent.CreationTime, consent.Expiration);
        }

        /// <summary>
        /// Gets the user consent asynchronous.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            var key = GetConsentKey(subjectId, clientId);
            return GetItemAsync(key);
        }

        /// <summary>
        /// Removes the user consent asynchronous.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            var key = GetConsentKey(subjectId, clientId);
            return RemoveItemAsync(key);
        }
    }
}
