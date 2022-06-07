// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;

namespace IdentityServer4.ResponseHandling
{
    /// <summary>
    /// Indicates interaction outcome for user on authorization endpoint.
    /// </summary>
    public class InteractionResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user must login.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is login; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user must consent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is consent; otherwise, <c>false</c>.
        /// </value>
        public bool IsConsent { get; set; }

        /// <summary>
        /// Gets a value indicating whether the result is an error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError => Error != null;

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user must be redirected to a custom page.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is redirect; otherwise, <c>false</c>.
        /// </value>
        public bool IsRedirect => RedirectUrl.IsPresent();

        /// <summary>
        /// Gets or sets the URL for the custom page.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }
    }
}
