// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace IdentityServer4.Configuration
{
    /// <summary>
    /// Options for Mutual TLS features
    /// </summary>
    public class MutualTlsOptions
    {
        /// <summary>
        /// Specifies if MTLS support should be enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Specifies the name of the authentication handler for X.509 client certificates
        /// </summary>
        public string ClientCertificateAuthenticationScheme { get; set; } = "Certificate";

        /// <summary>
        /// Specifies a separate domain to run the MTLS endpoints on.
        /// If the string does not contain any dots, a subdomain is assumed - e.g. main domain: identityserver.local, MTLS domain: mtls.identityserver.local
        /// If the string contains dots, a completely separate domain is assumend, e.g. main domain: identity.app.com, MTLS domain: mtls.app.com. In this case you must set a static issuer name on the options.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Specifies whether a cnf claim gets emitted for access tokens if a client certificate was present.
        /// Normally the cnf claims only gets emitted if the client used the client certificate for authentication,
        /// setting this to true, will set the claim regardless of the authentication method. (defaults to false).
        /// </summary>
        public bool AlwaysEmitConfirmationClaim { get; set; }
    }
}
