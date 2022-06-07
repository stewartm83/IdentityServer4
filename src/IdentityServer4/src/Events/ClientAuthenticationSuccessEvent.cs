﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for successful client authentication
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class ClientAuthenticationSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAuthenticationSuccessEvent"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="authenticationMethod">The authentication method.</param>
        public ClientAuthenticationSuccessEvent(string clientId, string authenticationMethod)
            : base(EventCategories.Authentication, 
                  "Client Authentication Success",
                  EventTypes.Success, 
                  EventIds.ClientAuthenticationSuccess)
        {
            ClientId = clientId;
            AuthenticationMethod = authenticationMethod;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the authentication method.
        /// </summary>
        /// <value>
        /// The authentication method.
        /// </value>
        public string AuthenticationMethod { get; set; }
    }
}
