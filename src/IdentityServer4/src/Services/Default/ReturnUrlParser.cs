// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Parses a return URL using all registered URL parsers
    /// </summary>
    public class ReturnUrlParser
    {
        private readonly IEnumerable<IReturnUrlParser> _parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnUrlParser"/> class.
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        public ReturnUrlParser(IEnumerable<IReturnUrlParser> parsers)
        {
            _parsers = parsers;
        }

        /// <summary>
        /// Parses the return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public virtual async Task<AuthorizationRequest> ParseAsync(string returnUrl)
        {
            foreach (var parser in _parsers)
            {
                var result = await parser.ParseAsync(returnUrl);
                if (result != null)
                {
                    return result;
                }
            }

            return null;            
        }

        /// <summary>
        /// Determines whether a return URL is valid.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        ///   <c>true</c> if the return URL is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValidReturnUrl(string returnUrl)
        {
            foreach (var parser in _parsers)
            {
                if (parser.IsValidReturnUrl(returnUrl))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
