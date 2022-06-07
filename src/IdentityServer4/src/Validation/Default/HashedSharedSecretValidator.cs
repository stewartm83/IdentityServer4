// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates a shared secret stored in SHA256 or SHA512
    /// </summary>
    public class HashedSharedSecretValidator : ISecretValidator
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSharedSecretValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public HashedSharedSecretValidator(ILogger<HashedSharedSecretValidator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates a secret
        /// </summary>
        /// <param name="secrets">The stored secrets.</param>
        /// <param name="parsedSecret">The received secret.</param>
        /// <returns>
        /// A validation result
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Id or cedential</exception>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.SharedSecret)
            {
                _logger.LogDebug("Hashed shared secret validator cannot process {type}", parsedSecret.Type ?? "null");
                return fail;
            }

            var sharedSecrets = secrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret);
            if (!sharedSecrets.Any())
            {
                _logger.LogDebug("No shared secret configured for client.");
                return fail;
            }

            var sharedSecret = parsedSecret.Credential as string;

            if (parsedSecret.Id.IsMissing() || sharedSecret.IsMissing())
            {
                throw new ArgumentException("Id or Credential is missing.");
            }

            var secretSha256 = sharedSecret.Sha256();
            var secretSha512 = sharedSecret.Sha512();

            foreach (var secret in sharedSecrets)
            {
                var secretDescription = string.IsNullOrEmpty(secret.Description) ? "no description" : secret.Description;

                bool isValid = false;
                byte[] secretBytes;

                try
                {
                    secretBytes = Convert.FromBase64String(secret.Value);
                }
                catch (FormatException)
                {
                    _logger.LogInformation("Secret: {description} uses invalid hashing algorithm.", secretDescription);
                    return fail;
                }
                catch (ArgumentNullException)
                {
                    _logger.LogInformation("Secret: {description} is null.", secretDescription);
                    return fail;
                }

                if (secretBytes.Length == 32)
                {
                    isValid = TimeConstantComparer.IsEqual(secret.Value, secretSha256);
                }
                else if (secretBytes.Length == 64)
                {
                    isValid = TimeConstantComparer.IsEqual(secret.Value, secretSha512);
                }
                else
                {
                    _logger.LogInformation("Secret: {description} uses invalid hashing algorithm.", secretDescription);
                    return fail;
                }

                if (isValid)
                {
                    return success;
                }
            }

            _logger.LogDebug("No matching hashed secret found.");
            return fail;
        }
    }
}
