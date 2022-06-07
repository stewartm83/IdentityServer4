// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer4
{
    /// <summary>
    /// Model properties of an IdentityServer user
    /// </summary>
    public class IdentityServerUser
    {
        /// <summary>
        /// Subject ID (mandatory)
        /// </summary>
        public string SubjectId { get; }

        /// <summary>
        /// Display name (optional)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Identity provider (optional)
        /// </summary>
        public string IdentityProvider { get; set; }

        /// <summary>
        /// Authentication methods
        /// </summary>
        public ICollection<string> AuthenticationMethods { get; set; } = new HashSet<string>();

        /// <summary>
        /// Authentication time
        /// </summary>
        public DateTime? AuthenticationTime { get; set; }

        /// <summary>
        /// Additional claims
        /// </summary>
        public ICollection<Claim> AdditionalClaims { get; set; } = new HashSet<Claim>(new ClaimComparer());

        /// <summary>
        /// Initializes a user identity
        /// </summary>
        /// <param name="subjectId">The subject ID</param>
        public IdentityServerUser(string subjectId)
        {
            if (subjectId.IsMissing()) throw new ArgumentException("SubjectId is mandatory", nameof(subjectId));

            SubjectId = subjectId;
        }

        /// <summary>
        /// Creates an IdentityServer claims principal
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ClaimsPrincipal CreatePrincipal()
        {
            if (SubjectId.IsMissing()) throw new ArgumentException("SubjectId is mandatory", nameof(SubjectId));
            var claims = new List<Claim> { new Claim(JwtClaimTypes.Subject, SubjectId) };

            if (DisplayName.IsPresent())
            {
                claims.Add(new Claim(JwtClaimTypes.Name, DisplayName));
            }

            if (IdentityProvider.IsPresent())
            {
                claims.Add(new Claim(JwtClaimTypes.IdentityProvider, IdentityProvider));
            }

            if (AuthenticationTime.HasValue)
            {
                claims.Add(new Claim(JwtClaimTypes.AuthenticationTime, new DateTimeOffset(AuthenticationTime.Value).ToUnixTimeSeconds().ToString()));
            }

            if (AuthenticationMethods.Any())
            {
                foreach (var amr in AuthenticationMethods)
                {
                    claims.Add(new Claim(JwtClaimTypes.AuthenticationMethod, amr));
                }
            }

            claims.AddRange(AdditionalClaims);

            var id = new ClaimsIdentity(claims.Distinct(new ClaimComparer()), Constants.IdentityServerAuthenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role);
            return new ClaimsPrincipal(id);
        }
    }
}
