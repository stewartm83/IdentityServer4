// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Implementation of strict redirect URI validator that allows a random port if 127.0.0.1 is used.
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.StrictRedirectUriValidator" />
    public class StrictRedirectUriValidatorAppAuth : StrictRedirectUriValidator
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictRedirectUriValidatorAppAuth"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StrictRedirectUriValidatorAppAuth(ILogger<StrictRedirectUriValidatorAppAuth> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Determines whether a redirect URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            var isAllowed = await base.IsRedirectUriValidAsync(requestedUri, client);
            if (isAllowed) return isAllowed;

            // since this is appauth specific, we can require pkce
            if (client.RequirePkce && client.RedirectUris.Contains("http://127.0.0.1")) return IsLoopback(requestedUri);

            return false;
        }

        /// <summary>
        /// Determines whether a post logout URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            var isAllowed = await base.IsPostLogoutRedirectUriValidAsync(requestedUri, client);
            if (isAllowed) return isAllowed;

            // since this is appauth specific, we can require pkce
            if (client.RequirePkce && client.RedirectUris.Contains("http://127.0.0.1")) return IsLoopback(requestedUri);

            return false;
        }

        /// <summary>
        /// Check if <paramref name="requestedUri"/> is of the form http://127.0.0.1:port/path.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="requestedUri"/> is a valid Loopback URI; <c>false</c> otherwise.
        /// </returns>
        internal bool IsLoopback(string requestedUri)
        {
            _logger.LogDebug("Checking for 127.0.0.1 redirect URI");

            // Validate that the requestedUri is not null or empty.
            if (string.IsNullOrEmpty(requestedUri))
            {
                _logger.LogDebug("'requestedUri' is null or empty");
                return false;
            }

            var parts = requestedUri.Split(':');

            if (parts.Length != 3)
            {
                _logger.LogDebug("invalid format - http://127.0.0.1:port is required.");
                return false;
            }

            if (!string.Equals(parts[0], "http", StringComparison.Ordinal) ||
                !string.Equals(parts[1], "//127.0.0.1", StringComparison.Ordinal))
            {
                _logger.LogDebug("invalid format - http://127.0.0.1:port is required.");
                return false;
            }

            string portAsString;
            int indexOfPathSeparator = parts[2].IndexOfAny(new[] { '/', '?', '#' });
            if (indexOfPathSeparator > 0)
            {
                portAsString = parts[2].Substring(0, indexOfPathSeparator);
            }
            else
            {
                portAsString = parts[2];
            }

            // Valid port range is 0 through 65535.
            if (int.TryParse(portAsString, out var port))
            {
                if (port >= 0 && port < 65536)
                {
                    return true;
                }
            }

            _logger.LogDebug("invalid port");
            return false;
        }
    }
}
