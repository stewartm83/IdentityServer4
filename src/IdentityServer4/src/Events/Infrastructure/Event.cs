// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Logging;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Models base class for events raised from IdentityServer.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event" /> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">category</exception>
        protected Event(string category, string name, EventTypes type, int id, string message = null)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            EventType = type;
            Id = id;
            Message = message;
        }

        /// <summary>
        /// Allows implementing custom initialization logic.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task PrepareAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public EventTypes EventType { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the event message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the per-request activity identifier.
        /// </summary>
        /// <value>
        /// The activity identifier.
        /// </value>
        public string ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the time stamp when the event was raised.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the server process identifier.
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the local ip address of the current request.
        /// </summary>
        /// <value>
        /// The local ip address.
        /// </value>
        public string LocalIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the remote ip address of the current request.
        /// </summary>
        /// <value>
        /// The remote ip address.
        /// </value>
        public string RemoteIpAddress { get; set; }

        /// <summary>
        /// Obfuscates a token.
        /// </summary>
        /// <param name="value">The token.</param>
        /// <returns></returns>
        protected static string Obfuscate(string value)
        {
            var last4Chars = "****";
            if (value.IsPresent() && value.Length > 4)
            {
                last4Chars = value.Substring(value.Length - 4);
            }

            return "****" + last4Chars;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}
