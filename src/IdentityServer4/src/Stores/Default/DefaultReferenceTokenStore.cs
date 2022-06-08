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
    /// Default reference token store.
    /// </summary>
    public class DefaultReferenceTokenStore : DefaultGrantStore<Token>, IReferenceTokenStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultReferenceTokenStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handleGenerationService">The handle generation service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultReferenceTokenStore(
            IPersistedGrantStore store, 
            IPersistentGrantSerializer serializer,
            IHandleGenerationService handleGenerationService,
            ILogger<DefaultReferenceTokenStore> logger) 
            : base(IdentityServerConstants.PersistedGrantTypes.ReferenceToken, store, serializer, handleGenerationService, logger)
        {
        }

        /// <summary>
        /// Stores the reference token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Task<string> StoreReferenceTokenAsync(Token token)
        {
            return CreateItemAsync(token, token.ClientId, token.SubjectId, token.SessionId, token.Description, token.CreationTime, token.Lifetime);
        }

        /// <summary>
        /// Gets the reference token asynchronous.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns></returns>
        public Task<Token> GetReferenceTokenAsync(string handle)
        {
            return GetItemAsync(handle);
        }

        /// <summary>
        /// Removes the reference token asynchronous.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns></returns>
        public Task RemoveReferenceTokenAsync(string handle)
        {
            return RemoveItemAsync(handle);
        }

        /// <summary>
        /// Removes the reference tokens asynchronous.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveReferenceTokensAsync(string subjectId, string clientId)
        {
            return RemoveAllAsync(subjectId, clientId);
        }
    }
}
