// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Endpoints.Results
{
    /// <summary>
    /// Result for the jwks document
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointResult" />
    public class JsonWebKeysResult : IEndpointResult
    {
        /// <summary>
        /// Gets the web keys.
        /// </summary>
        /// <value>
        /// The web keys.
        /// </value>
        public IEnumerable<JsonWebKey> WebKeys { get; }

        /// <summary>
        /// Gets the maximum age.
        /// </summary>
        /// <value>
        /// The maximum age.
        /// </value>
        public int? MaxAge { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebKeysResult" /> class.
        /// </summary>
        /// <param name="webKeys">The web keys.</param>
        /// <param name="maxAge">The maximum age.</param>
        public JsonWebKeysResult(IEnumerable<JsonWebKey> webKeys, int? maxAge)
        {
            WebKeys = webKeys ?? throw new ArgumentNullException(nameof(webKeys));
            MaxAge = maxAge;
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public Task ExecuteAsync(HttpContext context)
        {
            if (MaxAge.HasValue && MaxAge.Value >= 0)
            {
                context.Response.SetCache(MaxAge.Value, "Origin");
            }

            return context.Response.WriteJsonAsync(new { keys = WebKeys }, "application/json; charset=UTF-8");
        }
    }
}
