// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using System;
using System.Security.Claims;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Context describing the is-active check
    /// </summary>
    public class IsActiveContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsActiveContext"/> class.
        /// </summary>
        public IsActiveContext(ClaimsPrincipal subject, Client client, string caller)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (caller.IsMissing()) throw new ArgumentNullException(nameof(caller));

            Subject = subject;
            Client = client;
            Caller = caller;
            
            IsActive = true;
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public ClaimsPrincipal Subject { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public Client Client { get; set; }

        /// <summary>
        /// Gets or sets the caller.
        /// </summary>
        /// <value>
        /// The caller.
        /// </value>
        public string Caller { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subject is active and can recieve tokens.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the subject is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
    }
}
