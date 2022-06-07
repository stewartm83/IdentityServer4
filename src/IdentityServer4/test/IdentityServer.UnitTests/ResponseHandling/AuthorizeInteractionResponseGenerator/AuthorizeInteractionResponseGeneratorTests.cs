// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;
using static IdentityModel.OidcConstants;

namespace IdentityServer.UnitTests.ResponseHandling.AuthorizeInteractionResponseGenerator
{
    public class AuthorizeInteractionResponseGeneratorTests
    {
        private IdentityServerOptions _options = new IdentityServerOptions();
        private IdentityServer4.ResponseHandling.AuthorizeInteractionResponseGenerator _subject;
        private MockConsentService _mockConsentService = new MockConsentService();
        private StubClock _clock = new StubClock();

        public AuthorizeInteractionResponseGeneratorTests()
        {
            _subject = new IdentityServer4.ResponseHandling.AuthorizeInteractionResponseGenerator(
                _clock,
                TestLogger.Create<IdentityServer4.ResponseHandling.AuthorizeInteractionResponseGenerator>(),
                _mockConsentService,
                new MockProfileService());
        }


        [Fact]
        public async Task Authenticated_User_with_restricted_current_Idp_with_prompt_none_must_error()
        {
            var request = new ValidatedAuthorizeRequest
            {
                ClientId = "foo",
                Subject = new IdentityServerUser("123")
                {
                    IdentityProvider = IdentityServerConstants.LocalIdentityProvider
                }.CreatePrincipal(),
                Client = new Client
                {
                    EnableLocalLogin = false,
                    IdentityProviderRestrictions = new List<string>
                    {
                        "some_idp"
                    }
                },
                PromptModes = new[] { PromptModes.None },
            };

            var result = await _subject.ProcessInteractionAsync(request);

            result.IsError.Should().BeTrue();
            result.IsLogin.Should().BeFalse();
        }

        [Fact]
        public async Task Authenticated_User_with_maxage_with_prompt_none_must_error()
        {
            _clock.UtcNowFunc = () => new DateTime(2020, 02, 03, 9, 0, 0);

            var request = new ValidatedAuthorizeRequest
            {
                ClientId = "foo",
                Subject = new IdentityServerUser("123")
                {
                    AuthenticationTime = new DateTime(2020, 02, 01, 9, 0, 0),
                    IdentityProvider = IdentityServerConstants.LocalIdentityProvider
                }.CreatePrincipal(),
                Client = new Client
                {
                    EnableLocalLogin = true,
                },
                PromptModes = new[] { PromptModes.None },
                MaxAge = 3600
            };

            var result = await _subject.ProcessInteractionAsync(request);

            result.IsError.Should().BeTrue();
            result.IsLogin.Should().BeFalse();
        }

        [Fact]
        public async Task Authenticated_User_with_different_requested_Idp_with_prompt_none_must_error()
        {
            var request = new ValidatedAuthorizeRequest
            {
                ClientId = "foo",
                Client = new Client(),
                AuthenticationContextReferenceClasses = new List<string>{
                    "idp:some_idp"
                },
                Subject = new IdentityServerUser("123")
                {
                    IdentityProvider = IdentityServerConstants.LocalIdentityProvider
                }.CreatePrincipal(),
                PromptModes = new[] { PromptModes.None }
            };

            var result = await _subject.ProcessInteractionAsync(request);

            result.IsError.Should().BeTrue();
            result.IsLogin.Should().BeFalse();
        }

        [Fact]
        public async Task Authenticated_User_beyond_client_user_sso_lifetime_with_prompt_none_should_error()
        {
            var request = new ValidatedAuthorizeRequest
            {
                ClientId = "foo",
                Client = new Client()
                {
                    UserSsoLifetime = 3600 // 1h
                },
                Subject = new IdentityServerUser("123")
                {
                    IdentityProvider = "local",
                    AuthenticationTime = _clock.UtcNow.UtcDateTime.Subtract(TimeSpan.FromSeconds(3700))
                }.CreatePrincipal(),
                PromptModes = new[] { PromptModes.None }
            };

            var result = await _subject.ProcessInteractionAsync(request);

            result.IsError.Should().BeTrue();
            result.IsLogin.Should().BeFalse();
        }

        [Fact]
        public async Task locally_authenticated_user_but_client_does_not_allow_local_with_prompt_none_should_error()
        {
            var request = new ValidatedAuthorizeRequest
            {
                ClientId = "foo",
                Client = new Client()
                {
                    EnableLocalLogin = false
                },
                Subject = new IdentityServerUser("123")
                {
                    IdentityProvider = IdentityServerConstants.LocalIdentityProvider
                }.CreatePrincipal(),
                PromptModes = new[] { PromptModes.None }
            };

            var result = await _subject.ProcessInteractionAsync(request);

            result.IsError.Should().BeTrue();
            result.IsLogin.Should().BeFalse();
        }
    }
}
