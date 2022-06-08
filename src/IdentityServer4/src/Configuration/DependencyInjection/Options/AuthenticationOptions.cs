// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Configures the login and logout views and behavior.
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        /// Sets the cookie authentication scheme configured by the host used for interactive users. If not set, the scheme will inferred from the host's default authentication scheme.
        /// This setting is typically used when AddPolicyScheme is used in the host as the default scheme.
        /// </summary>
        public string CookieAuthenticationScheme { get; set; }

        /// <summary>
        /// Sets the cookie lifetime (only effective if the IdentityServer-provided cookie handler is used)
        /// </summary>
        public TimeSpan CookieLifetime { get; set; } = Constants.DefaultCookieTimeSpan;

        /// <summary>
        /// Specified if the cookie should be sliding or not (only effective if the built-in cookie middleware is used)
        /// </summary>
        public bool CookieSlidingExpiration { get; set; } = false;
        
        /// <summary>
        /// Specifies the SameSite mode for the internal authentication and temp cookie
        /// </summary>
        public SameSiteMode CookieSameSiteMode { get; set; } = SameSiteMode.None;

        /// <summary>
        /// Indicates if user must be authenticated to accept parameters to end session endpoint. Defaults to false.
        /// </summary>
        /// <value>
        /// <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public bool RequireAuthenticatedUserForSignOutMessage { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the cookie used for the check session endpoint.
        /// </summary>
        public string CheckSessionCookieName { get; set; } = IdentityServerConstants.DefaultCheckSessionCookieName;
        
        /// <summary>
        /// Gets or sets the domain of the cookie used for the check session endpoint. Defaults to null.
        /// </summary>
        public string CheckSessionCookieDomain { get; set; }

        /// <summary>
        /// Gets or sets the SameSite mode of the cookie used for the check session endpoint. Defaults to SameSiteMode.None.
        /// </summary>
        public SameSiteMode CheckSessionCookieSameSiteMode { get; set; } = SameSiteMode.None;

        /// <summary>
        /// If set, will require frame-src CSP headers being emitting on the end session callback endpoint which renders iframes to clients for front-channel signout notification.
        /// </summary>
        public bool RequireCspFrameSrcForSignout { get; set; } = true;
    }
}
