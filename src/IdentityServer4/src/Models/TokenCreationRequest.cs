// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Validation;
using System;
using System.Security.Claims;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models the data to create a token from a validated request.
    /// </summary>
    public class TokenCreationRequest
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public ClaimsPrincipal Subject { get; set; }

        /// <summary>
        /// Gets or sets the validated resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public ResourceValidationResult ValidatedResources { get; set; }

        /// <summary>
        /// Gets or sets the validated request.
        /// </summary>
        /// <value>
        /// The validated request.
        /// </value>
        public ValidatedRequest ValidatedRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include all identity claims].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include all identity claims]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeAllIdentityClaims { get; set; }

        /// <summary>
        /// Gets or sets the access token to hash.
        /// </summary>
        /// <value>
        /// The access token to hash.
        /// </value>
        public string AccessTokenToHash { get; set; }

        /// <summary>
        /// Gets or sets the authorization code to hash.
        /// </summary>
        /// <value>
        /// The authorization code to hash.
        /// </value>
        public string AuthorizationCodeToHash { get; set; }

        /// <summary>
        /// Gets or sets pre-hashed state
        /// </summary>
        /// <value>
        /// The pre-hashed state
        /// </value>
        public string StateHash { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>
        /// The nonce.
        /// </value>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Called to validate the <see cref="TokenCreationRequest"/> before it is processed.
        /// </summary>
        public void Validate()
        {
            if (ValidatedResources == null) throw new ArgumentNullException(nameof(ValidatedResources));
            if (ValidatedRequest == null) throw new ArgumentNullException(nameof(ValidatedRequest));
        }
    }
}
