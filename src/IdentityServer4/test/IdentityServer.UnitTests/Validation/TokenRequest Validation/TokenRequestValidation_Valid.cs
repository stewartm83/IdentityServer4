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
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Validation.TokenRequest_Validation
{
    public class TokenRequestValidation_Valid
    {
        private const string Category = "TokenRequest Validation - General - Valid";

        private IClientStore _clients = Factory.CreateClientStore();

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ResourceOwner_password_for_user_with_no_password_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob_no_password");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
            result.ValidatedRequest.UserName.Should().Be("bob_no_password");
        }
        
        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_code_request_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("codeclient");
            var grants = Factory.CreateAuthorizationCodeStore();

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                RequestedScopes = new List<string>
                {
                    "openid"
                }
            };

            var handle = await grants.StoreAuthorizationCodeAsync(code);

            var validator = Factory.CreateTokenRequestValidator(
                authorizationCodeStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode);
            parameters.Add(OidcConstants.TokenRequest.Code, handle);
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_code_request_with_refresh_token_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("codeclient");
            var grants = Factory.CreateAuthorizationCodeStore();

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                RedirectUri = "https://server/cb",
                RequestedScopes = new List<string>
                {
                    "openid",
                    "offline_access"
                }
            };

            var handle = await grants.StoreAuthorizationCodeAsync(code);

            var validator = Factory.CreateTokenRequestValidator(
                authorizationCodeStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode);
            parameters.Add(OidcConstants.TokenRequest.Code, handle);
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_client_credentials_request_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_client_credentials_request_with_default_scopes_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client_restricted");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
            

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_client_credentials_request_for_implicit_and_client_credentials_client_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("implicit_and_client_creds_client");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_client_credentials_request_restricted_client_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client_restricted");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_resource_owner_request_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_resource_wwner_request_with_refresh_token_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource offline_access");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_resource_owner_request_restricted_client_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient_restricted");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task valid_extension_grant_request_should_succeed()
        {
            var client = await _clients.FindEnabledClientByIdAsync("customgrantclient");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "custom_grant");
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_refresh_token_request_should_succeed()
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
                refreshTokenStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_refresh_token_request_using_restricted_client_should_succeed()
        {
            var subjectClaim = new Claim(JwtClaimTypes.Subject, "foo");

            var refreshToken = new RefreshToken
            {
                AccessToken = new Token("access_token")
                {
                    Claims = new List<Claim> { subjectClaim },
                    ClientId = "roclient_restricted_refresh"
                },

                Lifetime = 600,
                CreationTime = DateTime.UtcNow
            };

            var grants = Factory.CreateRefreshTokenStore();
            var handle = await grants.StoreRefreshTokenAsync(refreshToken);

            var client = await _clients.FindEnabledClientByIdAsync("roclient_restricted_refresh");

            var validator = Factory.CreateTokenRequestValidator(
                refreshTokenStore: grants);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "refresh_token");
            parameters.Add(OidcConstants.TokenRequest.RefreshToken, handle);

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }
        
        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_device_code_request_should_succeed()
        {
            var deviceCode = new DeviceCode
            {
                ClientId = "device_flow",
                IsAuthorized = true,
                Subject = new IdentityServerUser("bob").CreatePrincipal(),
                IsOpenId = true,
                Lifetime = 300,
                CreationTime = DateTime.UtcNow,
                AuthorizedScopes = new[] { "openid", "profile", "resource" }
            };

            var client = await _clients.FindClientByIdAsync("device_flow");

            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection
            {
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.DeviceCode},
                {"device_code", Guid.NewGuid().ToString()}
            };

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());
            result.IsError.Should().BeFalse();
        }
    }
}
