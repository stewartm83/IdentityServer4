// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Validation;
using static IdentityServer4.Constants;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for failed token issuance
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class TokenIssuedFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="description">The description.</param>
        public TokenIssuedFailureEvent(ValidatedAuthorizeRequest request, string error, string description)
            : this()
        {
            if (request != null)
            {
                ClientId = request.ClientId;
                ClientName = request.Client?.ClientName;
                RedirectUri = request.RedirectUri;
                Scopes = request.RequestedScopes?.ToSpaceSeparatedString();
                GrantType = request.GrantType;

                if (request.Subject != null && request.Subject.Identity.IsAuthenticated)
                {
                    SubjectId = request.Subject?.GetSubjectId();
                }
            }

            Endpoint = EndpointNames.Authorize;
            Error = error;
            ErrorDescription = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public TokenIssuedFailureEvent(TokenRequestValidationResult result)
            : this()
        {
            if (result.ValidatedRequest != null)
            {
                ClientId = result.ValidatedRequest.Client.ClientId;
                ClientName = result.ValidatedRequest.Client.ClientName;
                GrantType = result.ValidatedRequest.GrantType;
                Scopes = result.ValidatedRequest.RequestedScopes?.ToSpaceSeparatedString();

                if (result.ValidatedRequest.Subject != null && result.ValidatedRequest.Subject.Identity.IsAuthenticated)
                {
                    SubjectId = result.ValidatedRequest.Subject.GetSubjectId();
                }
            }

            Endpoint = EndpointNames.Token;
            Error = result.Error;
            ErrorDescription = result.ErrorDescription;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        protected TokenIssuedFailureEvent()
            : base(EventCategories.Token,
                  "Token Issued Failure",
                  EventTypes.Failure,
                  EventIds.TokenIssuedFailure)
        {
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
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string Scopes { get; set; }

        /// <summary>
        /// Gets or sets the grant type.
        /// </summary>
        /// <value>
        /// The grant type.
        /// </value>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; set; }
    }
}
