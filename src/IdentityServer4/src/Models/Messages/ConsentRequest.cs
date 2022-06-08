// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models the parameters to identify a request for consent.
    /// </summary>
    public class ConsentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentRequest"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="subject">The subject.</param>
        public ConsentRequest(AuthorizationRequest request, string subject)
        {
            ClientId = request.Client.ClientId;
            Nonce = request.Parameters[OidcConstants.AuthorizeRequest.Nonce];
            ScopesRequested = request.Parameters[OidcConstants.AuthorizeRequest.Scope].ParseScopesString();
            Subject = subject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentRequest"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="subject">The subject.</param>
        public ConsentRequest(NameValueCollection parameters, string subject)
        {
            ClientId = parameters[OidcConstants.AuthorizeRequest.ClientId];
            Nonce = parameters[OidcConstants.AuthorizeRequest.Nonce];
            ScopesRequested = parameters[OidcConstants.AuthorizeRequest.Scope].ParseScopesString();
            Subject = subject;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>
        /// The nonce.
        /// </value>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the scopes requested.
        /// </summary>
        /// <value>
        /// The scopes requested.
        /// </value>
        public IEnumerable<string> ScopesRequested { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get
            {
                var normalizedScopes = ScopesRequested?.OrderBy(x => x).Distinct().Aggregate((x, y) => x + "," + y);
                var value = $"{ClientId}:{Subject}:{Nonce}:{normalizedScopes}";

                using (var sha = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(value);
                    var hash = sha.ComputeHash(bytes);

                    return Base64Url.Encode(hash);
                }
            }
        }
    }
}
