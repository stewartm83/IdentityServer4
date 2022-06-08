// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using System.Collections.Specialized;
using System.Security.Claims;
using IdentityModel;
using System.Linq;
using System;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Base class for a validate authorize or token request
    /// </summary>
    public class ValidatedRequest
    {
        /// <summary>
        /// Gets or sets the raw request data
        /// </summary>
        /// <value>
        /// The raw.
        /// </value>
        public NameValueCollection Raw { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public Client Client { get; set; }

        /// <summary>
        /// Gets or sets the secret used to authenticate the client.
        /// </summary>
        /// <value>
        /// The parsed secret.
        /// </value>
        public ParsedSecret Secret { get; set; }

        /// <summary>
        /// Gets or sets the effective access token lifetime for the current request.
        /// This value is initally read from the client configuration but can be modified in the request pipeline
        /// </summary>
        public int AccessTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the client claims for the current request.
        /// This value is initally read from the client configuration but can be modified in the request pipeline
        /// </summary>
        public ICollection<Claim> ClientClaims { get; set; } = new HashSet<Claim>(new ClaimComparer());

        /// <summary>
        /// Gets or sets the effective access token type for the current request.
        /// This value is initally read from the client configuration but can be modified in the request pipeline
        /// </summary>
        public AccessTokenType AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public ClaimsPrincipal Subject { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionId { get; set; }
        
        /// <summary>
        /// Gets or sets the identity server options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public IdentityServerOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the validated resources for the request.
        /// </summary>
        /// <value>
        /// The validated resources.
        /// </value>
        public ResourceValidationResult ValidatedResources { get; set; } = new ResourceValidationResult();

        /// <summary>
        /// Gets or sets the value of the confirmation method (will become the cnf claim). Must be a JSON object.
        /// </summary>
        /// <value>
        /// The confirmation.
        /// </value>
        public string Confirmation { get; set; }

        /// <summary>
        /// Gets or sets the client ID that should be used for the current request (this is useful for token exchange scenarios)
        /// </summary>
        /// <value>
        /// The client ID
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Sets the client and the appropriate request specific settings.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="secret">The client secret (optional).</param>
        /// <param name="confirmation">The confirmation.</param>
        /// <exception cref="ArgumentNullException">client</exception>
        public void SetClient(Client client, ParsedSecret secret = null, string confirmation = "")
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Secret = secret;
            Confirmation = confirmation;
            ClientId = client.ClientId;

            AccessTokenLifetime = client.AccessTokenLifetime;
            AccessTokenType = client.AccessTokenType;
            ClientClaims = client.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)).ToList();
        }
    }
}
