// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Extension methods for client.
    /// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Returns true if the client is an implicit-only client.
        /// </summary>
        public static bool IsImplicitOnly(this Client client)
        {
            return client != null &&
                client.AllowedGrantTypes != null &&
                client.AllowedGrantTypes.Count == 1 &&
                client.AllowedGrantTypes.First() == GrantType.Implicit;
        }

        /// <summary>
        /// Constructs a list of SecurityKey from a Secret collection
        /// </summary>
        /// <param name="secrets">The secrets</param>
        /// <returns></returns>
        public static Task<List<SecurityKey>> GetKeysAsync(this IEnumerable<Secret> secrets)
        {
            var secretList = secrets.ToList().AsReadOnly();
            var keys = new List<SecurityKey>();

            var certificates = GetCertificates(secretList)
                                .Select(c => (SecurityKey)new X509SecurityKey(c))
                                .ToList();
            keys.AddRange(certificates);

            var jwks = secretList
                        .Where(s => s.Type == IdentityServerConstants.SecretTypes.JsonWebKey)
                        .Select(s => new Microsoft.IdentityModel.Tokens.JsonWebKey(s.Value))
                        .ToList();
            keys.AddRange(jwks);

            return Task.FromResult(keys);
        }

        private static List<X509Certificate2> GetCertificates(IEnumerable<Secret> secrets)
        {
            return secrets
                .Where(s => s.Type == IdentityServerConstants.SecretTypes.X509CertificateBase64)
                .Select(s => new X509Certificate2(Convert.FromBase64String(s.Value)))
                .Where(c => c != null)
                .ToList();
        }
    }
}
