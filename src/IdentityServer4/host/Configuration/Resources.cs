// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServerHost.Configuration
{
    public class Resources
    {
        // identity resources represent identity data about a user that can be requested via the scope parameter (OpenID Connect)
        public static readonly IEnumerable<IdentityResource> IdentityResources =
            new[]
            {
                // some standard scopes from the OIDC spec
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                // custom identity resource with some consolidated claims
                new IdentityResource("custom.profile", new[] { JwtClaimTypes.Name, JwtClaimTypes.Email, "location", JwtClaimTypes.Address })
            };

        // API scopes represent values that describe scope of access and can be requested by the scope parameter (OAuth)
        public static readonly IEnumerable<ApiScope> ApiScopes =
            new[]
            {
                // local API scope
                new ApiScope(LocalApi.ScopeName),

                // resource specific scopes
                new ApiScope("resource1.scope1"),
                new ApiScope("resource2.scope1"), 
                
                // a scope without resource association
                new ApiScope("scope3"),
                
                // a scope shared by multiple resources
                new ApiScope("shared.scope"),

                // a parameterized scope
                new ApiScope("transaction", "Transaction")
                {
                    Description = "Some Transaction"
                }
            };

        // API resources are more formal representation of a resource with processing rules and their scopes (if any)
        public static readonly IEnumerable<ApiResource> ApiResources = 
            new[]
            {
                new ApiResource("resource1", "Resource 1")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) },

                    Scopes = { "resource1.scope1", "shared.scope" }
                },
                
                new ApiResource("resource2", "Resource 2")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // additional claims to put into access token
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    },

                    Scopes = { "resource2.scope1", "shared.scope" }
                }
            };
    }
}
