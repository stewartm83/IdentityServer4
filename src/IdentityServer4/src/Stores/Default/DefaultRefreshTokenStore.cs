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
    /// Default refresh token store.
    /// </summary>
    public class DefaultRefreshTokenStore : DefaultGrantStore<RefreshToken>, IRefreshTokenStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRefreshTokenStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handleGenerationService">The handle generation service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultRefreshTokenStore(
            IPersistedGrantStore store, 
            IPersistentGrantSerializer serializer, 
            IHandleGenerationService handleGenerationService,
            ILogger<DefaultRefreshTokenStore> logger) 
            : base(IdentityServerConstants.PersistedGrantTypes.RefreshToken, store, serializer, handleGenerationService, logger)
        {
        }

        /// <summary>
        /// Stores the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        public async Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            return await CreateItemAsync(refreshToken, refreshToken.ClientId, refreshToken.SubjectId, refreshToken.SessionId, refreshToken.Description, refreshToken.CreationTime, refreshToken.Lifetime);
        }

        /// <summary>
        /// Updates the refresh token.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        public Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            return StoreItemAsync(handle, refreshToken, refreshToken.ClientId, refreshToken.SubjectId, refreshToken.SessionId, refreshToken.Description, refreshToken.CreationTime, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime), refreshToken.ConsumedTime);
        }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <param name="refreshTokenHandle">The refresh token handle.</param>
        /// <returns></returns>
        public Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            return GetItemAsync(refreshTokenHandle);
        }

        /// <summary>
        /// Removes the refresh token.
        /// </summary>
        /// <param name="refreshTokenHandle">The refresh token handle.</param>
        /// <returns></returns>
        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            return RemoveItemAsync(refreshTokenHandle);
        }

        /// <summary>
        /// Removes the refresh tokens.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            return RemoveAllAsync(subjectId, clientId);
        }
    }
}
