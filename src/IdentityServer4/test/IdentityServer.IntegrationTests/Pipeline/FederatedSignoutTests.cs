﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.IntegrationTests.Common;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace IdentityServer.IntegrationTests.Pipeline
{
    public class FederatedSignoutTests
    {
        private const string Category = "Federated Signout";

        private IdentityServerPipeline _pipeline = new IdentityServerPipeline();
        private ClaimsPrincipal _user;

        public FederatedSignoutTests()
        {
            _user = new IdentityServerUser("bob")
            {
                AdditionalClaims = { new Claim(JwtClaimTypes.SessionId, "123") }
            }.CreatePrincipal();

            _pipeline = new IdentityServerPipeline();

            _pipeline.IdentityScopes.AddRange(new IdentityResource[] {
                new IdentityResources.OpenId()
            });

            _pipeline.Clients.Add(new Client
            {
                ClientId = "client1",
                AllowedGrantTypes = GrantTypes.Implicit,
                RequireConsent = false,
                AllowedScopes = new List<string> { "openid" },
                RedirectUris = new List<string> { "https://client1/callback" },
                FrontChannelLogoutUri = "https://client1/signout",
                PostLogoutRedirectUris = new List<string> { "https://client1/signout-callback" },
                AllowAccessTokensViaBrowser = true
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
        public async Task valid_request_to_federated_signout_endpoint_should_render_page_with_iframe()
        {
            await _pipeline.LoginAsync(_user);

            await _pipeline.RequestAuthorizationEndpointAsync(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("text/html");
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Contain("https://server/connect/endsession/callback?endSessionId=");
        }

        [Fact]
        public async Task valid_POST_request_to_federated_signout_endpoint_should_render_page_with_iframe()
        {
            await _pipeline.LoginAsync(_user);

            await _pipeline.RequestAuthorizationEndpointAsync(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            var response = await _pipeline.BrowserClient.PostAsync(IdentityServerPipeline.FederatedSignOutUrl, new FormUrlEncodedContent(new Dictionary<string, string> { { "sid", "123" } }));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("text/html");
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Contain("https://server/connect/endsession/callback?endSessionId=");
        }

        [Fact]
        public async Task no_clients_signed_into_should_not_render_page_with_iframe()
        {
            await _pipeline.LoginAsync(_user);

            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task no_authenticated_user_should_not_render_page_with_iframe()
        {
            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task user_not_signed_out_should_not_render_page_with_iframe()
        {
            _pipeline.OnFederatedSignout = ctx =>
            {
                return Task.FromResult(true);
            };

            await _pipeline.LoginAsync(_user);

            await _pipeline.RequestAuthorizationEndpointAsync(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }

        [Fact]
        public async Task non_200_should_not_render_page_with_iframe()
        {
            _pipeline.OnFederatedSignout = async ctx =>
            {
                await ctx.SignOutAsync(); // even if we signout, we should not see iframes
                ctx.Response.Redirect("http://foo");
                return true;
            };

            await _pipeline.LoginAsync(_user);

            await _pipeline.RequestAuthorizationEndpointAsync(
                clientId: "client1",
                responseType: "id_token",
                scope: "openid",
                redirectUri: "https://client1/callback",
                state: "123_state",
                nonce: "123_nonce");

            _pipeline.BrowserClient.AllowAutoRedirect = false;
            var response = await _pipeline.BrowserClient.GetAsync(IdentityServerPipeline.FederatedSignOutUrl + "?sid=123");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Content.Headers.ContentType.Should().BeNull();
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Be(String.Empty);
        }
    }
}
