// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using System.Linq;
using System;
using IdentityServer4.Extensions;

namespace IdentityServer4.Services
{
    /// <summary>
    /// The default key material service
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IKeyMaterialService" />
    public class DefaultKeyMaterialService : IKeyMaterialService
    {
        private readonly IEnumerable<ISigningCredentialStore> _signingCredentialStores;
        private readonly IEnumerable<IValidationKeysStore> _validationKeysStores;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKeyMaterialService"/> class.
        /// </summary>
        /// <param name="validationKeysStores">The validation keys stores.</param>
        /// <param name="signingCredentialStores">The signing credential store.</param>
        public DefaultKeyMaterialService(IEnumerable<IValidationKeysStore> validationKeysStores, IEnumerable<ISigningCredentialStore> signingCredentialStores)
        {
            _signingCredentialStores = signingCredentialStores;
            _validationKeysStores = validationKeysStores;
        }

        /// <inheritdoc/>
        public async Task<SigningCredentials> GetSigningCredentialsAsync(IEnumerable<string> allowedAlgorithms = null)
        {
            if (_signingCredentialStores.Any())
            {
                if (allowedAlgorithms.IsNullOrEmpty())
                {
                    return await _signingCredentialStores.First().GetSigningCredentialsAsync();
                }

                var credential = (await GetAllSigningCredentialsAsync()).FirstOrDefault(c => allowedAlgorithms.Contains(c.Algorithm));
                if (credential is null)
                {
                    throw new InvalidOperationException($"No signing credential for algorithms ({allowedAlgorithms.ToSpaceSeparatedString()}) registered.");
                }

                return credential;
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SigningCredentials>> GetAllSigningCredentialsAsync()
        {
            var credentials = new List<SigningCredentials>();

            foreach (var store in _signingCredentialStores)
            {
                credentials.Add(await store.GetSigningCredentialsAsync());
            }

            return credentials;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var keys = new List<SecurityKeyInfo>();

            foreach (var store in _validationKeysStores)
            {
                keys.AddRange(await store.GetValidationKeysAsync());
            }

            return keys;
        }
    }
}
