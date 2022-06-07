// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for successful token revocation
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class TokenRevokedSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevokedSuccessEvent"/> class.
        /// </summary>
        /// <param name="requestResult">The request result.</param>
        /// <param name="client">The client.</param>
        public TokenRevokedSuccessEvent(TokenRevocationRequestValidationResult requestResult, Client client)
            : base(EventCategories.Token,
                  "Token Revoked Success",
                  EventTypes.Success,
                  EventIds.TokenRevokedSuccess)
        {
            ClientId = client.ClientId;
            ClientName = client.ClientName;
            TokenType = requestResult.TokenTypeHint;
            Token = Obfuscate(requestResult.Token);
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
    }
}
