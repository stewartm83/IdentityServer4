// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Validation.TokenRequest_Validation
{
    public class TokenRequestValidation_RefreshToken_Invalid
    {
        private const string Category = "TokenRequest Validation - RefreshToken - Invalid";

        private IClientStore _clients = Factory.CreateClientStore();

        [Fact]
        [Trait("Category", Category)]
        public async Task Non_existing_RefreshToken()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, "nonexistent");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task RefreshTokenTooLong()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var options = new IdentityServerOptions();

            var validator = Factory.CreateTokenRequestValidator();
            var longRefreshToken = "x".Repeat(options.InputLengthRestrictions.RefreshToken + 1);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, longRefreshToken);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Expired_RefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token("access_token") { ClientId = "roclient" },
                Lifetime = 10,
                CreationTime = DateTime.UtcNow.AddSeconds(-15)
            };

            var grants = Factory.CreateRefreshTokenStore();
            var handle = await grants.StoreRefreshTokenAsync(refreshToken);

            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator(
                refreshTokenStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Wrong_Client_Binding_RefreshToken_Request()
        {
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token("access_token")
                {
                    ClientId = "otherclient",
                    Lifetime = 600,
                    CreationTime = DateTime.UtcNow
                }
            };

            var grants = Factory.CreateRefreshTokenStore();
            var handle = await grants.StoreRefreshTokenAsync(refreshToken);

            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator(
                refreshTokenStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Client_has_no_OfflineAccess_Scope_anymore_at_RefreshToken_Request()
        {
            var refreshToken = new RefreshToken
            {
                AccessToken = new Token("access_token")
                {
                    ClientId = "roclient_restricted"
                },
                Lifetime = 600,
                CreationTime = DateTime.UtcNow
            };

            var grants = Factory.CreateRefreshTokenStore();
            var handle = await grants.StoreRefreshTokenAsync(refreshToken);

            var client = await _clients.FindEnabledClientByIdAsync("roclient_restricted");

            var validator = Factory.CreateTokenRequestValidator(
                refreshTokenStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task RefreshToken_Request_with_disabled_User()
        {
            var subjectClaim = new Claim(JwtClaimTypes.Subject, "foo");

            var refreshToken = new RefreshToken
            {
                AccessToken = new Token("access_token")
                {
                    Claims = new List<Claim> { subjectClaim },
                    ClientId = "roclient"
                },
                Lifetime = 600,
                CreationTime = DateTime.UtcNow
            };

            var grants = Factory.CreateRefreshTokenStore();
            var handle = await grants.StoreRefreshTokenAsync(refreshToken);

            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator(
                refreshTokenStore: grants,
                profile: new TestProfileService(shouldBeActive: false));

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }
    }
}
