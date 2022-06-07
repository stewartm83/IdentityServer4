// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



using System;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Base class for data that needs to be written out as cookies.
    /// </summary>
    public class Message<TModel>
    {
        /// <summary>
        /// Should only be used from unit tests
        /// </summary>
        /// <param name="data"></param>
        internal Message(TModel data) : this(data, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// For JSON serializer. 
        /// System.Text.Json.JsonSerializer requires public, parameterless constructor
        /// </summary>
        public Message()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TModel}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="now">The current UTC date/time.</param>
        public Message(TModel data, DateTime now)
        {
            Created = now.Ticks;
            Data = data;
        }

        /// <summary>
        /// Gets or sets the UTC ticks the <see cref="Message{TModel}" /> was created.
        /// </summary>
        /// <value>
        /// The created UTC ticks.
        /// </value>
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public TModel Data { get; set; }
    }
}
