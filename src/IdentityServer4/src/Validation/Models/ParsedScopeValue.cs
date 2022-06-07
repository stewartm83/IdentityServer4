// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Models a parsed scope value.
    /// </summary>
    public class ParsedScopeValue
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rawValue"></param>
        public ParsedScopeValue(string rawValue)
            : this(rawValue, rawValue, null)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="parsedName"></param>
        /// <param name="parsedParameter"></param>
        public ParsedScopeValue(string rawValue, string parsedName, string parsedParameter)
        {
            if (String.IsNullOrWhiteSpace(rawValue))
            {
                throw new ArgumentNullException(nameof(rawValue));
            }

            if (String.IsNullOrWhiteSpace(parsedName))
            {
                throw new ArgumentNullException(nameof(parsedName));
            }

            RawValue = rawValue;
            ParsedName = parsedName;
            ParsedParameter = parsedParameter;
        }

        /// <summary>
        /// The original (raw) value of the scope.
        /// </summary>
        public string RawValue { get; set; }

        /// <summary>
        /// The parsed name of the scope. If the scope has no structure, the parsed name will be the same as the raw value.
        /// </summary>
        public string ParsedName { get; set; }

        // future: maybe this should be something w/ more structure? dictionary?
        /// <summary>
        /// The parameter value of the parsed scope. If the scope has no structure, then the value will be null.
        /// </summary>
        public string ParsedParameter { get; set; }
    }
}
