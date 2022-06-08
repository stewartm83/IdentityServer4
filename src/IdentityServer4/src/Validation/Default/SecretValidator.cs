// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates secrets using the registered validators
    /// </summary>
    public class SecretValidator : ISecretsListValidator
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<ISecretValidator> _validators;
        private readonly ISystemClock _clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretValidator"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="validators">The validators.</param>
        /// <param name="logger">The logger.</param>
        public SecretValidator(ISystemClock clock, IEnumerable<ISecretValidator> validators, ILogger<ISecretsListValidator> logger)
        {
            _clock = clock;
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// Validates the secret.
        /// </summary>
        /// <param name="parsedSecret">The parsed secret.</param>
        /// <param name="secrets">The secrets.</param>
        /// <returns></returns>
        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var secretsArray = secrets as Secret[] ?? secrets.ToArray();

            var expiredSecrets = secretsArray.Where(s => s.Expiration.HasExpired(_clock.UtcNow.UtcDateTime)).ToList();
            if (expiredSecrets.Any())
            {
                expiredSecrets.ForEach(
                    ex => _logger.LogInformation("Secret [{description}] is expired", ex.Description ?? "no description"));
            }

            var currentSecrets = secretsArray.Where(s => !s.Expiration.HasExpired(_clock.UtcNow.UtcDateTime)).ToArray();

            // see if a registered validator can validate the secret
            foreach (var validator in _validators)
            {
                var secretValidationResult = await validator.ValidateAsync(currentSecrets, parsedSecret);

                if (secretValidationResult.Success)
                {
                    _logger.LogDebug("Secret validator success: {0}", validator.GetType().Name);
                    return secretValidationResult;
                }
            }

            _logger.LogDebug("Secret validators could not validate secret");
            return new SecretValidationResult { Success = false };
        }
    }
}
