// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Configuration;

namespace IdentityServer4
{
    /// <summary>
    /// Extensions for IdentityServerTools
    /// </summary>
    public static class IdentityServerToolsExtensions
    {
        /// <summary>
        /// Issues the client JWT.
        /// </summary>
        /// <param name="tools">The tools.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="audiences">The audiences.</param>
        /// <param name="additionalClaims">Additional claims</param>
        /// <returns></returns>
        public static async Task<string> IssueClientJwtAsync(this IdentityServerTools tools,
            string clientId,
            int lifetime,
            IEnumerable<string> scopes = null,
            IEnumerable<string> audiences = null,
            IEnumerable<Claim> additionalClaims = null)
        {
            var claims = new HashSet<Claim>(new ClaimComparer());
            var context = tools.ContextAccessor.HttpContext;
            var options = context.RequestServices.GetRequiredService<IdentityServerOptions>();
            
            if (additionalClaims != null)
            {
                foreach (var claim in additionalClaims)
                {
                    claims.Add(claim);
                }
            }

            claims.Add(new Claim(JwtClaimTypes.ClientId, clientId));

            if (!scopes.IsNullOrEmpty())
            {
                foreach (var scope in scopes)
                {
                    claims.Add(new Claim(JwtClaimTypes.Scope, scope));
                }
            }

            if (options.EmitStaticAudienceClaim)
            {
                claims.Add(new Claim(JwtClaimTypes.Audience, string.Format(IdentityServerConstants.AccessTokenAudience, tools.ContextAccessor.HttpContext.GetIdentityServerIssuerUri().EnsureTrailingSlash())));
            }
            
            if (!audiences.IsNullOrEmpty())
            {
                foreach (var audience in audiences)
                {
                    claims.Add(new Claim(JwtClaimTypes.Audience, audience));
                }
            }

            return await tools.IssueJwtAsync(lifetime, claims);
        }
    }
}
