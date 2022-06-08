// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models the user's response to the consent screen.
    /// </summary>
    public class ConsentResponse
    {
        /// <summary>
        /// Error, if any, for the consent response.
        /// </summary>
        public AuthorizationError? Error { get; set; }

        /// <summary>
        /// Error description.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets if consent was granted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if consent was granted; otherwise, <c>false</c>.
        /// </value>
        public bool Granted => ScopesValuesConsented != null && ScopesValuesConsented.Any() && Error == null;

        /// <summary>
        /// Gets or sets the scope values consented to.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public IEnumerable<string> ScopesValuesConsented { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user wishes the consent to be remembered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if consent is to be remembered; otherwise, <c>false</c>.
        /// </value>
        public bool RememberConsent { get; set; }

        /// <summary>
        /// Gets the description of the device.
        /// </summary>
        /// <value>
        /// The description of the device.
        /// </value>
        public string Description { get; set; }
    }

    /// <summary>
    /// Enum to model interaction authorization errors.
    /// </summary>
    public enum AuthorizationError
    {
        /// <summary>
        /// Access denied
        /// </summary>
        AccessDenied,

        /// <summary>
        /// Interaction required
        /// </summary>
        InteractionRequired,

        /// <summary>
        /// Login required
        /// </summary>
        LoginRequired,

        /// <summary>
        /// Account selection required
        /// </summary>
        AccountSelectionRequired,

        /// <summary>
        /// Consent required
        /// </summary>
        ConsentRequired
    }

}
