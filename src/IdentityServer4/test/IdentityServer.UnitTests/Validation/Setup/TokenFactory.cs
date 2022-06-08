// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;

namespace IdentityServer.UnitTests.Validation.Setup
{
    internal static class TokenFactory
    {
        public static Token CreateAccessToken(Client client, string subjectId, int lifetime, params string[] scopes)
        {
            var claims = new List<Claim> 
            {
                new Claim("client_id", client.ClientId),
                new Claim("sub", subjectId)
            };

            scopes.ToList().ForEach(s => claims.Add(new Claim("scope", s)));

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { "https://idsvr.com/resources" },
                Issuer = "https://idsvr.com",
                Lifetime = lifetime,
                Claims = claims,
                ClientId = client.ClientId,
                AccessTokenType = client.AccessTokenType
            };

            return token;
        }

        public static Token CreateAccessTokenLong(Client client, string subjectId, int lifetime, int count, params string[] scopes)
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", client.ClientId),
                new Claim("sub", subjectId)
            };

            for (int i = 0; i < count; i++)
            {
                claims.Add(new Claim("junk", "x".Repeat(100)));
            }

            scopes.ToList().ForEach(s => claims.Add(new Claim("scope", s)));

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { "https://idsvr.com/resources" },
                Issuer = "https://idsvr.com",
                Lifetime = lifetime,
                Claims = claims,
                ClientId = client.ClientId,
                AccessTokenType = client.AccessTokenType
            };

            return token;
        }

        public static Token CreateIdentityToken(string clientId, string subjectId)
        {
            var clients = Factory.CreateClientStore();

            var claims = new List<Claim> 
            {
                new Claim("sub", subjectId)
            };

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { clientId },
                ClientId = clientId,
                Issuer = "https://idsvr.com",
                Lifetime = 600,
                Claims = claims
            };

            return token;
        }

        public static Token CreateIdentityTokenLong(string clientId, string subjectId, int count)
        {
            var clients = Factory.CreateClientStore();

            var claims = new List<Claim>
            {
                new Claim("sub", subjectId)
            };

            for (int i = 0; i < count; i++)
            {
                claims.Add(new Claim("junk", "x".Repeat(100)));
            }

            var token = new Token(OidcConstants.TokenTypes.IdentityToken)
            {
                CreationTime = DateTime.UtcNow,
                Audiences = { clientId },
                ClientId = clientId,
                Issuer = "https://idsvr.com",
                Lifetime = 600,
                Claims = claims
            };

            return token;
        }
    }
}
