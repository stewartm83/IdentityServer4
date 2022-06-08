﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Configures events
    /// </summary>
    public class EventsOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to raise success events.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success event should be raised; otherwise, <c>false</c>.
        /// </value>
        public bool RaiseSuccessEvents { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to raise failure events.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failure events should be raised; otherwise, <c>false</c>.
        /// </value>
        public bool RaiseFailureEvents { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to raise information events.
        /// </summary>
        /// <value>
        /// <c>true</c> if information events should be raised; otherwise, <c>false</c>.
        /// </value>
        public bool RaiseInformationEvents { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to raise error events.
        /// </summary>
        /// <value>
        ///   <c>true</c> if error events should be raised; otherwise, <c>false</c>.
        /// </value>
        public bool RaiseErrorEvents { get; set; } = false;
    }
}
