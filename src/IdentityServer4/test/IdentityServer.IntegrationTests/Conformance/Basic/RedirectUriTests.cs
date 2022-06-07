// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Xunit;

namespace IdentityServer.IntegrationTests.Conformance.Basic
{
    public class RedirectUriTests
    {
        private const string Category = "Conformance.Basic.RedirectUriTests";

        private IdentityServerPipeline _mockPipeline = new IdentityServerPipeline();

        public RedirectUriTests()
        {
            _mockPipeline.Initialize();

            _mockPipeline.Clients.Add(new Client
            {
                Enabled = true,
                ClientId = "code_client",
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
                    "https://code_client/callback",
                    "https://code_client/callback?foo=bar&baz=quux"
                }
            });

            _mockPipeline.IdentityScopes.Add(new IdentityResources.OpenId());

            _mockPipeline.Users.Add(new TestUser
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
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Reject_redirect_uri_not_matching_registered_redirect_uri()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://bad",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorWasCalled.Should().BeTrue();
            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Reject_request_without_redirect_uri_when_multiple_registered()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                          clientId: "code_client",
                          responseType: "code",
                          scope: "openid",
                          // redirectUri deliberately absent 
                          redirectUri: null,
                          state: state,
                          nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorWasCalled.Should().BeTrue();
            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Preserves_query_parameters_in_redirect_uri()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            _mockPipeline.BrowserClient.AllowAutoRedirect = false;
            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_client/callback?foo=bar&baz=quux",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().StartWith("https://code_client/callback?");
            var authorization = _mockPipeline.ParseAuthorizationResponseUrl(response.Headers.Location.ToString());
            authorization.Code.Should().NotBeNull();
            authorization.State.Should().Be(state);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(response.Headers.Location.Query);
            query["foo"].ToString().Should().Be("bar");
            query["baz"].ToString().Should().Be("quux");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Rejects_redirect_uri_when_query_parameter_does_not_match()
        {
            await _mockPipeline.LoginAsync("bob");

            var nonce = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var url = _mockPipeline.CreateAuthorizeUrl(
                           clientId: "code_client",
                           responseType: "code",
                           scope: "openid",
                           redirectUri: "https://code_client/callback?baz=quux&foo=bar",
                           state: state,
                           nonce: nonce);
            var response = await _mockPipeline.BrowserClient.GetAsync(url);

            _mockPipeline.ErrorWasCalled.Should().BeTrue();
            _mockPipeline.ErrorMessage.Error.Should().Be("invalid_request");
        }
    }
}
