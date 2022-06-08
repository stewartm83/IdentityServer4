// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for unhandled exceptions
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class InvalidClientConfigurationEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionEvent" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="errorMessage">The error message.</param>
        public InvalidClientConfigurationEvent(Client client, string errorMessage)
            : base(EventCategories.Error,
                  "Invalid Client Configuration",
                  EventTypes.Error, 
                  EventIds.InvalidClientConfiguration,
                  errorMessage)
        {
            ClientId = client.ClientId;
            ClientName = client.ClientName ?? "unknown name";
        }

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName { get; set; }
    }
}
