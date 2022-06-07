// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// The introspection request validator
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IIntrospectionRequestValidator" />
    internal class IntrospectionRequestValidator : IIntrospectionRequestValidator
    {
        private readonly ILogger _logger;
        private readonly ITokenValidator _tokenValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionRequestValidator"/> class.
        /// </summary>
        /// <param name="tokenValidator">The token validator.</param>
        /// <param name="logger">The logger.</param>
        public IntrospectionRequestValidator(ITokenValidator tokenValidator, ILogger<IntrospectionRequestValidator> logger)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="api">The API.</param>
        /// <returns></returns>
        public async Task<IntrospectionRequestValidationResult> ValidateAsync(NameValueCollection parameters, ApiResource api)
        {
            _logger.LogDebug("Introspection request validation started.");

            // retrieve required token
            var token = parameters.Get("token");
            if (token == null)
            {
                _logger.LogError("Token is missing");

                return new IntrospectionRequestValidationResult
                {
                    IsError = true,
                    Api = api,
                    Error = "missing_token",
                    Parameters = parameters
                };
            }

            // validate token
            var tokenValidationResult = await _tokenValidator.ValidateAccessTokenAsync(token);

            // invalid or unknown token
            if (tokenValidationResult.IsError)
            {
                _logger.LogDebug("Token is invalid.");

                return new IntrospectionRequestValidationResult
                {
                    IsActive = false,
                    IsError = false,
                    Token = token,
                    Api = api,
                    Parameters = parameters
                };
            }

            _logger.LogDebug("Introspection request validation successful.");

            // valid token
            return new IntrospectionRequestValidationResult
            {
                IsActive = true,
                IsError = false,
                Token = token,
                Claims = tokenValidationResult.Claims,
                Api = api,
                Parameters = parameters
            };
        }
    }
}
