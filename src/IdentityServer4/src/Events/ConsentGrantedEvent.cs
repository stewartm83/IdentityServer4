// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for granted consent.
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class ConsentGrantedEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentGrantedEvent" /> class.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="requestedScopes">The requested scopes.</param>
        /// <param name="grantedScopes">The granted scopes.</param>
        /// <param name="consentRemembered">if set to <c>true</c> consent was remembered.</param>
        public ConsentGrantedEvent(string subjectId, string clientId, IEnumerable<string> requestedScopes, IEnumerable<string> grantedScopes, bool consentRemembered)
            : base(EventCategories.Grants,
                  "Consent granted",
                  EventTypes.Information,
                  EventIds.ConsentGranted)
        {
            SubjectId = subjectId;
            ClientId = clientId;
            RequestedScopes = requestedScopes;
            GrantedScopes = grantedScopes;
            ConsentRemembered = consentRemembered;
        }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the requested scopes.
        /// </summary>
        /// <value>
        /// The requested scopes.
        /// </value>
        public IEnumerable<string> RequestedScopes { get; set; }

        /// <summary>
        /// Gets or sets the granted scopes.
        /// </summary>
        /// <value>
        /// The granted scopes.
        /// </value>
        public IEnumerable<string> GrantedScopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether consent was remembered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if consent was remembered; otherwise, <c>false</c>.
        /// </value>
        public bool ConsentRemembered { get; set; }
    }
}
