// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class RevocationRequestValidation
    {
        private const string Category = "Revocation Request Validation Tests";

        private ITokenRevocationRequestValidator _validator;
        private Client _client;

        public RevocationRequestValidation()
        {
            _validator = new TokenRevocationRequestValidator(TestLogger.Create<TokenRevocationRequestValidator>());
            _client = new Client
            {
                ClientName = "Code Client",
                Enabled = true,
                ClientId = "codeclient",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },

                AllowedGrantTypes = GrantTypes.Code,

                RequireConsent = false,

                RedirectUris = new List<string>
                {
                    "https://server/cb"
                },

                AuthorizationCodeLifetime = 60
            };
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Parameters()
        {
            var parameters = new NameValueCollection();

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Token_Valid_Hint()
        {
            var parameters = new NameValueCollection
            {
                { "token_type_hint", "access_token" }
            };

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_And_AccessTokenHint()
        {
            var parameters = new NameValueCollection
            {
                { "token", "foo" },
                { "token_type_hint", "access_token" }
            };

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeFalse();
            result.Token.Should().Be("foo");
            result.TokenTypeHint.Should().Be("access_token");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_and_RefreshTokenHint()
        {
            var parameters = new NameValueCollection
            {
                { "token", "foo" },
                { "token_type_hint", "refresh_token" }
            };

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeFalse();
            result.Token.Should().Be("foo");
            result.TokenTypeHint.Should().Be("refresh_token");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_And_Missing_Hint()
        {
            var parameters = new NameValueCollection
            {
                { "token", "foo" }
            };

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeFalse();
            result.Token.Should().Be("foo");
            result.TokenTypeHint.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Token_And_Invalid_Hint()
        {
            var parameters = new NameValueCollection
            {
                { "token", "foo" },
                { "token_type_hint", "invalid" }
            };

            var result = await _validator.ValidateRequestAsync(parameters, _client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(Constants.RevocationErrors.UnsupportedTokenType);
        }
    }
}
