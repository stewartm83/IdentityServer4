// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validation result for token requests
    /// </summary>
    public class TokenRequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRequestValidationResult"/> class.
        /// </summary>
        /// <param name="validatedRequest">The validated request.</param>
        /// <param name="customResponse">The custom response.</param>
        public TokenRequestValidationResult(ValidatedTokenRequest validatedRequest, Dictionary<string, object> customResponse = null)
        {
            IsError = false;

            ValidatedRequest = validatedRequest;
            CustomResponse = customResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRequestValidationResult"/> class.
        /// </summary>
        /// <param name="validatedRequest">The validated request.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <param name="customResponse">The custom response.</param>
        public TokenRequestValidationResult(ValidatedTokenRequest validatedRequest, string error, string errorDescription = null, Dictionary<string, object> customResponse = null)
        {
            IsError = true;

            Error = error;
            ErrorDescription = errorDescription;
            ValidatedRequest = validatedRequest;
            CustomResponse = customResponse;
        }

        /// <summary>
        /// Gets the validated request.
        /// </summary>
        /// <value>
        /// The validated request.
        /// </value>
        public ValidatedTokenRequest ValidatedRequest { get; }

        /// <summary>
        /// Gets or sets the custom response.
        /// </summary>
        /// <value>
        /// The custom response.
        /// </value>
        public Dictionary<string, object> CustomResponse { get; set; }
    }
}
