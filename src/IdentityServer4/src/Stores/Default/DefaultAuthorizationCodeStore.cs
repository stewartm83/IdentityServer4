// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using IdentityServer4.Extensions;
using IdentityServer4.Services;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Default authorization code store.
    /// </summary>
    public class DefaultAuthorizationCodeStore : DefaultGrantStore<AuthorizationCode>, IAuthorizationCodeStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationCodeStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handleGenerationService">The handle generation service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultAuthorizationCodeStore(
            IPersistedGrantStore store,
            IPersistentGrantSerializer serializer,
            IHandleGenerationService handleGenerationService,
            ILogger<DefaultAuthorizationCodeStore> logger)
            : base(IdentityServerConstants.PersistedGrantTypes.AuthorizationCode, store, serializer, handleGenerationService, logger)
        {
        }

        /// <summary>
        /// Stores the authorization code asynchronous.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
        {
            return CreateItemAsync(code, code.ClientId, code.Subject.GetSubjectId(), code.SessionId, code.Description, code.CreationTime, code.Lifetime);
        }

        /// <summary>
        /// Gets the authorization code asynchronous.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            return GetItemAsync(code);
        }

        /// <summary>
        /// Removes the authorization code asynchronous.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Task RemoveAuthorizationCodeAsync(string code)
        {
            return RemoveItemAsync(code);
        }
    }
}
