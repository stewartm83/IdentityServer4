// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultConsentServiceTests
    {
        private DefaultConsentService _subject;
        private MockProfileService _mockMockProfileService = new MockProfileService();

        private ClaimsPrincipal _user;
        private Client _client;
        private TestUserConsentStore _userConsentStore = new TestUserConsentStore();
        private StubClock _clock = new StubClock();

        private DateTime now;

        public DefaultConsentServiceTests()
        {
            _clock.UtcNowFunc = () => UtcNow;

            _client = new Client
            {
                ClientId = "client",
                RequireConsent = true,
                RequirePkce = false
            };

            _user = new IdentityServerUser("bob")
            {
                AdditionalClaims =
                {
                    new Claim("foo", "foo1"),
                    new Claim("foo", "foo2"),
                    new Claim("bar", "bar1"),
                    new Claim("bar", "bar2"),
                    new Claim(JwtClaimTypes.AuthenticationContextClassReference, "acr1")
                }
            }.CreatePrincipal();

            _subject = new DefaultConsentService(_clock, _userConsentStore, TestLogger.Create<DefaultConsentService>());
        }

        public DateTime UtcNow
        {
            get
            {
                if (now > DateTime.MinValue) return now;
                return DateTime.UtcNow;
            }
        }

        [Fact]
        public async Task UpdateConsentAsync_when_client_does_not_allow_remember_consent_should_not_update_store()
        {
            _client.AllowRememberConsent = false;

            await _subject.UpdateConsentAsync(_user, _client, new [] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            var consent = await _userConsentStore.GetUserConsentAsync(_user.GetSubjectId(), _client.ClientId);
            consent.Should().BeNull();
        }

        [Fact]
        public async Task UpdateConsentAsync_should_persist_consent()
        {
            await _subject.UpdateConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            var consent = await _userConsentStore.GetUserConsentAsync(_user.GetSubjectId(), _client.ClientId);
            consent.Scopes.Count().Should().Be(2);
            consent.Scopes.Should().Contain("scope1");
            consent.Scopes.Should().Contain("scope2");
        }

        [Fact]
        public async Task UpdateConsentAsync_empty_scopes_should_should_remove_consent()
        {
            await _subject.UpdateConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            await _subject.UpdateConsentAsync(_user, _client, new ParsedScopeValue[] { });

            var consent = await _userConsentStore.GetUserConsentAsync(_user.GetSubjectId(), _client.ClientId);
            consent.Should().BeNull();
        }

        [Fact]
        public async Task RequiresConsentAsync_client_does_not_require_consent_should_not_require_consent()
        {
            _client.RequireConsent = false;

            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsentAsync_client_does_not_allow_remember_consent_should_require_consent()
        {
            _client.AllowRememberConsent = false;

            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_no_scopes_should_not_require_consent()
        {
            var result = await _subject.RequiresConsentAsync(_user, _client, new ParsedScopeValue[] { });

            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsentAsync_offline_access_should_require_consent()
        {
            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("offline_access") });

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_no_prior_consent_should_require_consent()
        {
            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_prior_consent_should_not_require_consent()
        {
            await _subject.UpdateConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsentAsync_prior_consent_with_more_scopes_should_not_require_consent()
        {
            await _subject.UpdateConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2"), new ParsedScopeValue("scope3") });

            var result = await _subject.RequiresConsentAsync(_user, _client, new [] { new ParsedScopeValue("scope2") });

            result.Should().BeFalse();
        }

        [Fact]
        public async Task RequiresConsentAsync_prior_consent_with_too_few_scopes_should_require_consent()
        {
            await _subject.UpdateConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope2"), new ParsedScopeValue("scope3") });

            var result = await _subject.RequiresConsentAsync(_user, _client, new[] { new ParsedScopeValue("scope1"), new ParsedScopeValue("scope2") });

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_expired_consent_should_require_consent()
        {
            now = DateTime.UtcNow;

            var scopes = new[] { new ParsedScopeValue("foo"), new ParsedScopeValue("bar") };
            _client.ConsentLifetime = 2;

            await _subject.UpdateConsentAsync(_user, _client, scopes);

            now = now.AddSeconds(3);

            var result = await _subject.RequiresConsentAsync(_user, _client, scopes);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RequiresConsentAsync_expired_consent_should_remove_consent()
        {
            now = DateTime.UtcNow;

            var scopes = new[] { new ParsedScopeValue("foo"), new ParsedScopeValue("bar") };
            _client.ConsentLifetime = 2;

            await _subject.UpdateConsentAsync(_user, _client, scopes);

            now = now.AddSeconds(3);

            await _subject.RequiresConsentAsync(_user, _client, scopes);

            var result = await _userConsentStore.GetUserConsentAsync(_user.GetSubjectId(), _client.ClientId);

            result.Should().BeNull();
        }
    }
}
