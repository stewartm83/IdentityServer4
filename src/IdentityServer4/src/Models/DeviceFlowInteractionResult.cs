// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace IdentityServer4.Models
{
    /// <summary>
    /// Request object for device flow interaction
    /// </summary>
    public class DeviceFlowInteractionResult
    {
        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access denied.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is access denied; otherwise, <c>false</c>.
        /// </value>
        public bool IsAccessDenied { get; set; }

        /// <summary>
        /// Create failure result
        /// </summary>
        /// <param name="errorDescription">The error description.</param>
        /// <returns></returns>
        public static DeviceFlowInteractionResult Failure(string errorDescription = null)
        {
            return new DeviceFlowInteractionResult
            {
                IsError = true,
                ErrorDescription = errorDescription
            };
        }
    }
}
