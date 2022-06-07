// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.EndSessionRequestValidation
{
    public class EndSessionRequestValidatorTests
    {
        private EndSessionRequestValidator _subject;
        private IdentityServerOptions _options;
        private StubTokenValidator _stubTokenValidator = new StubTokenValidator();
        private StubRedirectUriValidator _stubRedirectUriValidator = new StubRedirectUriValidator();
        private MockHttpContextAccessor _context = new MockHttpContextAccessor();
        private MockUserSession _userSession = new MockUserSession();
        private MockLogoutNotificationService _mockLogoutNotificationService = new MockLogoutNotificationService();
        private MockMessageStore<LogoutNotificationContext> _mockEndSessionMessageStore = new MockMessageStore<LogoutNotificationContext>();

        private ClaimsPrincipal _user;

        public EndSessionRequestValidatorTests()
        {
            _user = new IdentityServerUser("alice").CreatePrincipal();

            _options = TestIdentityServerOptions.Create();
            _subject = new EndSessionRequestValidator(
                _context,
                _options,
                _stubTokenValidator,
                _stubRedirectUriValidator,
                _userSession,
                _mockLogoutNotificationService,
                _mockEndSessionMessageStore,
                TestLogger.Create<EndSessionRequestValidator>());
        }

        [Fact]
        public async Task anonymous_user_when_options_require_authenticated_user_should_return_error()
        {
            _options.Authentication.RequireAuthenticatedUserForSignOutMessage = true;

            var parameters = new NameValueCollection();
            var result = await _subject.ValidateAsync(parameters, null);
            result.IsError.Should().BeTrue();

            result = await _subject.ValidateAsync(parameters, new ClaimsPrincipal());
            result.IsError.Should().BeTrue();

            result = await _subject.ValidateAsync(parameters, new ClaimsPrincipal(new ClaimsIdentity()));
            result.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task valid_params_should_return_success()
        {
            _stubTokenValidator.IdentityTokenValidationResult = new TokenValidationResult()
            {
                IsError = false,
                Claims = new Claim[] { new Claim("sub", _user.GetSubjectId()) },
                Client = new Client() { ClientId = "client"}
            };
            _stubRedirectUriValidator.IsPostLogoutRedirectUriValid = true;

            var parameters = new NameValueCollection();
            parameters.Add("id_token_hint", "id_token");
            parameters.Add("post_logout_redirect_uri", "http://client/signout-cb");
            parameters.Add("client_id", "client1");
            parameters.Add("state", "foo");

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeFalse();

            result.ValidatedRequest.Client.ClientId.Should().Be("client");
            result.ValidatedRequest.PostLogOutUri.Should().Be("http://client/signout-cb");
            result.ValidatedRequest.State.Should().Be("foo");
            result.ValidatedRequest.Subject.GetSubjectId().Should().Be(_user.GetSubjectId());
        }

        [Fact]
        public async Task no_post_logout_redirect_uri_should_not_use_single_registered_uri()
        {
            _stubTokenValidator.IdentityTokenValidationResult = new TokenValidationResult()
            {
                IsError = false,
                Claims = new Claim[] { new Claim("sub", _user.GetSubjectId()) },
                Client = new Client() { ClientId = "client1", PostLogoutRedirectUris = new List<string> { "foo" } }
            };
            _stubRedirectUriValidator.IsPostLogoutRedirectUriValid = true;

            var parameters = new NameValueCollection();
            parameters.Add("id_token_hint", "id_token");

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.PostLogOutUri.Should().BeNull();
        }

        [Fact]
        public async Task no_post_logout_redirect_uri_should_not_use_multiple_registered_uri()
        {
            _stubTokenValidator.IdentityTokenValidationResult = new TokenValidationResult()
            {
                IsError = false,
                Claims = new Claim[] { new Claim("sub", _user.GetSubjectId()) },
                Client = new Client() { ClientId = "client1", PostLogoutRedirectUris = new List<string> { "foo", "bar" } }
            };
            _stubRedirectUriValidator.IsPostLogoutRedirectUriValid = true;

            var parameters = new NameValueCollection();
            parameters.Add("id_token_hint", "id_token");

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.PostLogOutUri.Should().BeNull();
        }

        [Fact]
        public async Task post_logout_uri_fails_validation_should_not_honor_logout_uri()
        {
            _stubTokenValidator.IdentityTokenValidationResult = new TokenValidationResult()
            {
                IsError = false,
                Claims = new Claim[] { new Claim("sub", _user.GetSubjectId()) },
                Client = new Client() { ClientId = "client" }
            };
            _stubRedirectUriValidator.IsPostLogoutRedirectUriValid = false;

            var parameters = new NameValueCollection();
            parameters.Add("id_token_hint", "id_token");
            parameters.Add("post_logout_redirect_uri", "http://client/signout-cb");
            parameters.Add("client_id", "client1");
            parameters.Add("state", "foo");

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeFalse();

            result.ValidatedRequest.Client.ClientId.Should().Be("client");
            result.ValidatedRequest.Subject.GetSubjectId().Should().Be(_user.GetSubjectId());
            
            result.ValidatedRequest.State.Should().BeNull();
            result.ValidatedRequest.PostLogOutUri.Should().BeNull();
        }

        [Fact]
        public async Task subject_mismatch_should_return_error()
        {
            _stubTokenValidator.IdentityTokenValidationResult = new TokenValidationResult()
            {
                IsError = false,
                Claims = new Claim[] { new Claim("sub", "xoxo") },
                Client = new Client() { ClientId = "client" }
            };
            _stubRedirectUriValidator.IsPostLogoutRedirectUriValid = true;

            var parameters = new NameValueCollection();
            parameters.Add("id_token_hint", "id_token");
            parameters.Add("post_logout_redirect_uri", "http://client/signout-cb");
            parameters.Add("client_id", "client1");
            parameters.Add("state", "foo");

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeTrue();
        }

        [Fact]
        public async Task successful_request_should_return_inputs()
        {
            var parameters = new NameValueCollection();

            var result = await _subject.ValidateAsync(parameters, _user);
            result.IsError.Should().BeFalse();
            result.ValidatedRequest.Raw.Should().BeSameAs(parameters);
        }
    }
}
