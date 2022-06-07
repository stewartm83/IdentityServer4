// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using System.Collections.Generic;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for failed token introspection
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class TokenIntrospectionFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIntrospectionSuccessEvent" /> class.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="token">The token.</param>
        /// <param name="apiScopes">The API scopes.</param>
        /// <param name="tokenScopes">The token scopes.</param>
        public TokenIntrospectionFailureEvent(string apiName, string errorMessage, string token = null, IEnumerable<string> apiScopes = null, IEnumerable<string> tokenScopes = null)
            : base(EventCategories.Token,
                  "Token Introspection Failure",
                  EventTypes.Failure,
                  EventIds.TokenIntrospectionFailure,
                  errorMessage)
        {
            ApiName = apiName;

            if (token.IsPresent())
            {
                Token = Obfuscate(token);
            }

            if (apiScopes != null)
            {
                ApiScopes = apiScopes;
            }

            if (tokenScopes != null)
            {
                TokenScopes = tokenScopes;
            }
        }

        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        /// <value>
        /// The name of the API.
        /// </value>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the API scopes.
        /// </summary>
        /// <value>
        /// The API scopes.
        /// </value>
        public IEnumerable<string> ApiScopes { get; set; }

        /// <summary>
        /// Gets or sets the token scopes.
        /// </summary>
        /// <value>
        /// The token scopes.
        /// </value>
        public IEnumerable<string> TokenScopes { get; set; }
    }
}
