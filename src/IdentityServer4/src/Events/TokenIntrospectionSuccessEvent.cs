// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for successful token introspection
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class TokenIntrospectionSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIntrospectionSuccessEvent" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public TokenIntrospectionSuccessEvent(IntrospectionRequestValidationResult result)
            : base(EventCategories.Token,
                  "Token Introspection Success",
                  EventTypes.Success,
                  EventIds.TokenIntrospectionSuccess)
        {
            ApiName = result.Api.Name;
            IsActive = result.IsActive;

            if (result.Token.IsPresent())
            {
                Token = Obfuscate(result.Token);
            }
            
            if (!result.Claims.IsNullOrEmpty())
            {
                ClaimTypes = result.Claims.Select(c => c.Type).Distinct();
                TokenScopes = result.Claims.Where(c => c.Type == "scope").Select(c => c.Value);
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
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the claim types.
        /// </summary>
        /// <value>
        /// The claim types.
        /// </value>
        public IEnumerable<string> ClaimTypes { get; set; }

        /// <summary>
        /// Gets or sets the token scopes.
        /// </summary>
        /// <value>
        /// The token scopes.
        /// </value>
        public IEnumerable<string> TokenScopes { get; set; }
    }
}
