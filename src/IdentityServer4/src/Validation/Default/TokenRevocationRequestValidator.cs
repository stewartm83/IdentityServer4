// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// The token revocation request validator
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.ITokenRevocationRequestValidator" />
    internal class TokenRevocationRequestValidator : ITokenRevocationRequestValidator
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationRequestValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TokenRevocationRequestValidator(ILogger<TokenRevocationRequestValidator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameters
        /// or
        /// client
        /// </exception>
        public Task<TokenRevocationRequestValidationResult> ValidateRequestAsync(NameValueCollection parameters, Client client)
        {
            _logger.LogTrace("ValidateRequestAsync called");

            if (parameters == null)
            {
                _logger.LogError("no parameters passed");
                throw new ArgumentNullException(nameof(parameters));
            }

            if (client == null)
            {
                _logger.LogError("no client passed");
                throw new ArgumentNullException(nameof(client));
            }

            ////////////////////////////
            // make sure token is present
            ///////////////////////////
            var token = parameters.Get("token");
            if (token.IsMissing())
            {
                _logger.LogError("No token found in request");
                return Task.FromResult(new TokenRevocationRequestValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.TokenErrors.InvalidRequest
                });
            }

            var result = new TokenRevocationRequestValidationResult
            {
                IsError = false,
                Token = token,
                Client = client
            };

            ////////////////////////////
            // check token type hint
            ///////////////////////////
            var hint = parameters.Get("token_type_hint");
            if (hint.IsPresent())
            {
                if (Constants.SupportedTokenTypeHints.Contains(hint))
                {
                    _logger.LogDebug("Token type hint found in request: {tokenTypeHint}", hint);
                    result.TokenTypeHint = hint;
                }
                else
                {
                    _logger.LogError("Invalid token type hint: {tokenTypeHint}", hint);
                    result.IsError = true;
                    result.Error = Constants.RevocationErrors.UnsupportedTokenType;
                }
            }

            _logger.LogDebug("ValidateRequestAsync result: {validateRequestResult}", result);

            return Task.FromResult(result);
        }
    }
}
