// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates an extension grant request using the registered validators
    /// </summary>
    public class ExtensionGrantValidator
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IExtensionGrantValidator> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionGrantValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        /// <param name="logger">The logger.</param>
        public ExtensionGrantValidator(IEnumerable<IExtensionGrantValidator> validators, ILogger<ExtensionGrantValidator> logger)
        {
            if (validators == null)
            {
                _validators = Enumerable.Empty<IExtensionGrantValidator>();
            }
            else
            {
                _validators = validators;
            }

            _logger = logger;
        }

        /// <summary>
        /// Gets the available grant types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAvailableGrantTypes()
        {
            return _validators.Select(v => v.GrantType);
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<GrantValidationResult> ValidateAsync(ValidatedTokenRequest request)
        {
            var validator = _validators.FirstOrDefault(v => v.GrantType.Equals(request.GrantType, StringComparison.Ordinal));

            if (validator == null)
            {
                _logger.LogError("No validator found for grant type");
                return new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
            }

            try
            {
                _logger.LogTrace("Calling into custom grant validator: {type}", validator.GetType().FullName);

                var context = new ExtensionGrantValidationContext
                {
                    Request = request
                };
            
                await validator.ValidateAsync(context);
                return context.Result;
            }
            catch (Exception e)
            {
                _logger.LogError(1, e, "Grant validation error: {message}", e.Message);
                return new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
        }
    }
}
