// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class CodeFlowTests 
    {
        private const string Category = "Conformance.Basic.CodeFlowTests";

        private IdentityServerPipeline _pipeline = new IdentityServerPipeline();

        public CodeFlowTests()
        {
            _pipeline.IdentityScopes.Add(new IdentityResources.OpenId());
            _pipeline.Clients.Add(new Client
            {
                Enabled = true,
                ClientId = "code_pipeline.Client",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha512())
                },

                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = { "openid" },

                RequireConsent = false,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    "https://code_pipeline.Client/callback",
                    "https://code_pipeline.Client/callback?foo=bar&baz=quux"
                }
            });

            _pipeline.Users.Add(new TestUser
            {
                SubjectId = "bob",
                Username = "bob",
                Claims = new Claim[]
                   {
                        new Claim("name", "Bob Loblaw"),
                        new Claim("email", "bob@loblaw.com"),
                        new Claim("role", "Attorney")
                   }
            });

            _pipeline.Initialize();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task No_state_should_not_result_in_shash()
        {
            await _pipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();

            _pipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _pipeline.CreateAuthorizeUrl(
                           clientId: "code_pipeline.Client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_pipeline.Client/callback?foo=bar&baz=quux",
                           nonce: nonce);
            var response = await _pipeline.BrowserClient.GetAsync(url);

            var authorization = _pipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client
            var wrapper = new MessageHandlerWrapper(_pipeline.Handler);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",

                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback?foo=bar&baz=quux"
            });

            tokenResult.IsError.Should().BeFalse();
            tokenResult.HttpErrorReason.Should().Be("OK");
            tokenResult.TokenType.Should().Be("Bearer");
            tokenResult.AccessToken.Should().NotBeNull();
            tokenResult.ExpiresIn.Should().BeGreaterThan(0);
            tokenResult.IdentityToken.Should().NotBeNull();

            var token = new JwtSecurityToken(tokenResult.IdentityToken);
            
            var s_hash = token.Claims.FirstOrDefault(c => c.Type == "s_hash");
            s_hash.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task State_should_result_in_shash()
        {
            await _pipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();

            _pipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _pipeline.CreateAuthorizeUrl(
                           clientId: "code_pipeline.Client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_pipeline.Client/callback?foo=bar&baz=quux",
                           state: "state",
                           nonce: nonce);
            var response = await _pipeline.BrowserClient.GetAsync(url);

            var authorization = _pipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();

            var code = authorization.Code;

            // backchannel client
            var wrapper = new MessageHandlerWrapper(_pipeline.Handler);
            var tokenClient = new HttpClient(wrapper);
            var tokenResult = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = IdentityServerPipeline.TokenEndpoint,
                ClientId = "code_pipeline.Client",
                ClientSecret = "secret",

                Code = code,
                RedirectUri = "https://code_pipeline.Client/callback?foo=bar&baz=quux"
            });

            tokenResult.IsError.Should().BeFalse();
            tokenResult.HttpErrorReason.Should().Be("OK");
            tokenResult.TokenType.Should().Be("Bearer");
            tokenResult.AccessToken.Should().NotBeNull();
            tokenResult.ExpiresIn.Should().BeGreaterThan(0);
            tokenResult.IdentityToken.Should().NotBeNull();

            var token = new JwtSecurityToken(tokenResult.IdentityToken);
            
            var s_hash = token.Claims.FirstOrDefault(c => c.Type == "s_hash");
            s_hash.Should().NotBeNull();
            s_hash.Value.Should().Be(CryptoHelper.CreateHashClaimValue("state", "RS256"));
        }
    }
}
