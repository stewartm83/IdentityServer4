// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.TokenRequest_Validation
{
    public class TokenRequestValidation_ResourceOwner_Invalid
    {
        private const string Category = "TokenRequest Validation - ResourceOwner - Invalid";

        private IClientStore _clients = Factory.CreateClientStore();

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_GrantType_For_Client()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnauthorizedClient);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_Scope()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "unknown");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_Scope_Multiple()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource unknown");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Restricted_Scope()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient_restricted");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource2");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Restricted_Scope_Multiple()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient_restricted");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource resource2");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task No_ResourceOwnerCredentials()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ResourceOwner_UserName()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResourceOwner_Credentials()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "notbob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
            result.ErrorDescription.Should().Be("invalid_username_or_password");
        }
        
        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ResourceOwner_password_for_user_with_password_should_fail()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob_with_password");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Password_GrantType_Not_Supported()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator(resourceOwnerValidator: new NotSupportedResourceOwnerPasswordValidator(TestLogger.Create<NotSupportedResourceOwnerPasswordValidator>()));

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
            result.ErrorDescription.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Inactive_ResourceOwner()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator(profile: new TestProfileService(shouldBeActive: false));

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "bob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Password_GrantType_With_Custom_ErrorDescription()
        {
            var client = await _clients.FindEnabledClientByIdAsync("roclient");
            var validator = Factory.CreateTokenRequestValidator(resourceOwnerValidator: new TestResourceOwnerPasswordValidator(TokenRequestErrors.InvalidGrant, "custom error description"));

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            parameters.Add(OidcConstants.TokenRequest.Scope, "resource");
            parameters.Add(OidcConstants.TokenRequest.UserName, "bob");
            parameters.Add(OidcConstants.TokenRequest.Password, "notbob");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
            result.ErrorDescription.Should().Be("custom error description");
        }
    }
}
