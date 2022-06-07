// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.using System.Collections.Generic;

using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServerHost.Configuration
{
    public static class ClientsWeb
    {
        static string[] allowedScopes = 
        {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            IdentityServerConstants.StandardScopes.Email,
            "resource1.scope1", 
            "resource2.scope1",
            "transaction"
        };
        
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                ///////////////////////////////////////////
                // JS OIDC Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "js_oidc",
                    ClientName = "JavaScript OIDC Client",
                    ClientUri = "http://identityserver.io",
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    
                    RedirectUris = 
                    {
                        "https://localhost:44300/index.html",
                        "https://localhost:44300/callback.html",
                        "https://localhost:44300/silent.html",
                        "https://localhost:44300/popup.html"
                    },

                    PostLogoutRedirectUris = { "https://localhost:44300/index.html" },
                    AllowedCorsOrigins = { "https://localhost:44300" },

                    AllowedScopes = allowedScopes
                },
                
                ///////////////////////////////////////////
                // MVC Automatic Token Management Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "mvc.tokenmanagement",
                    
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AccessTokenLifetime = 75,

                    RedirectUris = { "https://localhost:44301/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44301/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44301/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowedScopes = allowedScopes
                },
                
                ///////////////////////////////////////////
                // MVC Code Flow Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "mvc.code",
                    ClientName = "MVC Code Flow",
                    ClientUri = "http://identityserver.io",

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RequireConsent = true,
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44302/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44302/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44302/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowedScopes = allowedScopes
                },
                
                ///////////////////////////////////////////
                // MVC Hybrid Flow Sample (Back Channel logout)
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "mvc.hybrid.backchannel",
                    ClientName = "MVC Hybrid (with BackChannel logout)",
                    ClientUri = "http://identityserver.io",

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce = false,

                    RedirectUris = { "https://localhost:44303/signin-oidc" },
                    BackChannelLogoutUri = "https://localhost:44303/logout",
                    PostLogoutRedirectUris = { "https://localhost:44303/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowedScopes = allowedScopes
                }
            };
        }
    }
}
