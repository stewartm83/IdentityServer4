// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Validation;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for device authorization failure
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class DeviceAuthorizationFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceAuthorizationFailureEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public DeviceAuthorizationFailureEvent(DeviceAuthorizationRequestValidationResult result)
            : this()
        {
            if (result.ValidatedRequest != null)
            {
                ClientId = result.ValidatedRequest.Client?.ClientId;
                ClientName = result.ValidatedRequest.Client?.ClientName;
                Scopes = result.ValidatedRequest.RequestedScopes?.ToSpaceSeparatedString();
                
            }

            Endpoint = Constants.EndpointNames.DeviceAuthorization;
            Error = result.Error;
            ErrorDescription = result.ErrorDescription;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceAuthorizationFailureEvent"/> class.
        /// </summary>
        public DeviceAuthorizationFailureEvent()
            : base(EventCategories.DeviceFlow,
                "Device Authorization Failure",
                EventTypes.Failure,
                EventIds.DeviceAuthorizationFailure)
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
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string Scopes { get; set; }
        
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
