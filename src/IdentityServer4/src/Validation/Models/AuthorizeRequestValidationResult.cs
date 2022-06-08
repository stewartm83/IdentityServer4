// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validation result for authorize requests
    /// </summary>
    public class AuthorizeRequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRequestValidationResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public AuthorizeRequestValidationResult(ValidatedAuthorizeRequest request)
        {
            ValidatedRequest = request;
            IsError = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRequestValidationResult" /> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        public AuthorizeRequestValidationResult(ValidatedAuthorizeRequest request, string error, string errorDescription = null)
        {
            ValidatedRequest = request;
            IsError = true;
            Error = error;
            ErrorDescription = errorDescription;
        }

        /// <summary>
        /// Gets or sets the validated request.
        /// </summary>
        /// <value>
        /// The validated request.
        /// </value>
        public ValidatedAuthorizeRequest ValidatedRequest { get; }
    }
}
