// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;

namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Caching options.
    /// </summary>
    public class CachingOptions
    {
        private static readonly TimeSpan Default = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets the client store expiration.
        /// </summary>
        /// <value>
        /// The client store expiration.
        /// </value>
        public TimeSpan ClientStoreExpiration { get; set; } = Default;

        /// <summary>
        /// Gets or sets the scope store expiration.
        /// </summary>
        /// <value>
        /// The scope store expiration.
        /// </value>
        public TimeSpan ResourceStoreExpiration { get; set; } = Default;

        /// <summary>
        /// Gets or sets the CORS origin expiration.
        /// </summary>
        public TimeSpan CorsExpiration { get; set; } = Default;
    }
}
