// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Default implementation of redirect URI validator. Validates the URIs against
    /// the client's configured URIs.
    /// </summary>
    public class StrictRedirectUriValidator : IRedirectUriValidator
    {
        /// <summary>
        /// Checks if a given URI string is in a collection of strings (using ordinal ignore case comparison)
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns></returns>
        protected bool StringCollectionContainsString(IEnumerable<string> uris, string requestedUri)
        {
            if (uris.IsNullOrEmpty()) return false;

            return uris.Contains(requestedUri, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a redirect URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public virtual Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        /// <summary>
        /// Determines whether a post logout URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public virtual Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }
    }
}
