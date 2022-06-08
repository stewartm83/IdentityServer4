// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;

namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Options class to configure discovery endpoint
    /// </summary>
    public class DiscoveryOptions
    {
        /// <summary>
        /// Show endpoints
        /// </summary>
        public bool ShowEndpoints { get; set; } = true;

        /// <summary>
        /// Show signing keys
        /// </summary>
        public bool ShowKeySet { get; set; } = true;

        /// <summary>
        /// Show identity scopes
        /// </summary>
        public bool ShowIdentityScopes { get; set; } = true;

        /// <summary>
        /// Show API scopes
        /// </summary>
        public bool ShowApiScopes { get; set; } = true;

        /// <summary>
        /// Show identity claims
        /// </summary>
        public bool ShowClaims { get; set; } = true;

        /// <summary>
        /// Show response types
        /// </summary>
        public bool ShowResponseTypes { get; set; } = true;

        /// <summary>
        /// Show response modes
        /// </summary>
        public bool ShowResponseModes { get; set; } = true;

        /// <summary>
        /// Show standard grant types
        /// </summary>
        public bool ShowGrantTypes { get; set; } = true;

        /// <summary>
        /// Show custom grant types
        /// </summary>
        public bool ShowExtensionGrantTypes { get; set; } = true;

        /// <summary>
        /// Show token endpoint authentication methods
        /// </summary>
        public bool ShowTokenEndpointAuthenticationMethods { get; set; } = true;

        /// <summary>
        /// Turns relative paths that start with ~/ into absolute paths
        /// </summary>
        public bool ExpandRelativePathsInCustomEntries { get; set; } = true;

        /// <summary>
        /// Sets the maxage value of the cache control header (in seconds) of the HTTP response. This gives clients a hint how often they should refresh their cached copy of the discovery document. If set to 0 no-cache headers will be set. Defaults to null, which does not set the header.
        /// </summary>
        /// <value>
        /// The cache interval in seconds.
        /// </value>
        public int? ResponseCacheInterval { get; set; } = null;

        /// <summary>
        /// Adds custom entries to the discovery document
        /// </summary>
        public Dictionary<string, object> CustomEntries { get; set; } = new Dictionary<string, object>();
    }
}
