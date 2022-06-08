﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models an authorization code.
    /// </summary>
    public class AuthorizationCode
    {
        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the life time in seconds.
        /// </summary>
        /// <value>
        /// The life time.
        /// </value>
        public int Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the client.
        /// </summary>
        /// <value>
        /// The ID of the client.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public ClaimsPrincipal Subject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this code is an OpenID Connect code.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is open identifier; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpenId { get; set; }
        
        /// <summary>
        /// Gets or sets the requested scopes.
        /// </summary>
        /// <value>
        /// The requested scopes.
        /// </value>
        // todo: brock, change to parsed scopes
        public IEnumerable<string> RequestedScopes { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>
        /// The nonce.
        /// </value>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the hashed state (to output s_hash claim).
        /// </summary>
        /// <value>
        /// The hashed state.
        /// </value>
        public string StateHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether consent was shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if consent was shown; otherwise, <c>false</c>.
        /// </value>
        public bool WasConsentShown { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the code challenge.
        /// </summary>
        /// <value>
        /// The code challenge.
        /// </value>
        public string CodeChallenge { get; set; }

        /// <summary>
        /// Gets or sets the code challenge method.
        /// </summary>
        /// <value>
        /// The code challenge method
        /// </value>
        public string CodeChallengeMethod { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets properties
        /// </summary>
        /// <value>
        /// The properties
        /// </value>
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
