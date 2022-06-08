// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class AccessTokenValidation
    {
        private const string Category = "Access token validation";

        private IClientStore _clients = Factory.CreateClientStore();
        private IdentityServerOptions _options = new IdentityServerOptions();
        private StubClock _clock = new StubClock();

        static AccessTokenValidation()
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        private DateTime now;
        public DateTime UtcNow
        {
            get
            {
                if (now > DateTime.MinValue) return now;
                return DateTime.UtcNow;
            }
        }

        public AccessTokenValidation()
        {
            _clock.UtcNowFunc = () => UtcNow;
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Reference_Token()
        {
            var store = Factory.CreateReferenceTokenStore();
            var validator = Factory.CreateTokenValidator(store);

            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write");

            var handle = await store.StoreReferenceTokenAsync(token);

            var result = await validator.ValidateAccessTokenAsync(handle);

            result.IsError.Should().BeFalse();
            result.Claims.Count().Should().Be(8);
            result.Claims.First(c => c.Type == JwtClaimTypes.ClientId).Value.Should().Be("roclient");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Reference_Token_with_required_Scope()
        {
            var store = Factory.CreateReferenceTokenStore();
            var validator = Factory.CreateTokenValidator(store);

            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write");

            var handle = await store.StoreReferenceTokenAsync(token);

            var result = await validator.ValidateAccessTokenAsync(handle, "read");

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Reference_Token_with_missing_Scope()
        {
            var store = Factory.CreateReferenceTokenStore();
            var validator = Factory.CreateTokenValidator(store);

            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write");

            var handle = await store.StoreReferenceTokenAsync(token);

            var result = await validator.ValidateAccessTokenAsync(handle, "missing");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InsufficientScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_Reference_Token()
        {
            var validator = Factory.CreateTokenValidator();

            var result = await validator.ValidateAccessTokenAsync("unknown");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Reference_Token_Too_Long()
        {
            var validator = Factory.CreateTokenValidator();
            var options = new IdentityServerOptions();

            var longToken = "x".Repeat(options.InputLengthRestrictions.TokenHandle + 1);
            var result = await validator.ValidateAccessTokenAsync(longToken);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Expired_Reference_Token()
        {
            now = DateTime.UtcNow;

            var store = Factory.CreateReferenceTokenStore();
            var validator = Factory.CreateTokenValidator(store, clock:_clock);

            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 2, "read", "write");
            token.CreationTime = now;

            var handle = await store.StoreReferenceTokenAsync(token);

            now = now.AddSeconds(3);

            var result = await validator.ValidateAccessTokenAsync(handle);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.ExpiredToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Malformed_JWT_Token()
        {
            var validator = Factory.CreateTokenValidator();

            var result = await validator.ValidateAccessTokenAsync("unk.nown");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_JWT_Token()
        {
            var signer = Factory.CreateDefaultTokenCreator();
            var jwt = await signer.CreateTokenAsync(TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write"));

            var validator = Factory.CreateTokenValidator(null);
            var result = await validator.ValidateAccessTokenAsync(jwt);

            result.IsError.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Trait("Category", Category)]
        public async Task JWT_Token_with_scopes_have_expected_claims(bool flag)
        {
            var options = TestIdentityServerOptions.Create();
            options.EmitScopesAsSpaceDelimitedStringInJwt = flag;
            
            var signer = Factory.CreateDefaultTokenCreator(options);
            var jwt = await signer.CreateTokenAsync(TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write"));

            var validator = Factory.CreateTokenValidator(null);
            var result = await validator.ValidateAccessTokenAsync(jwt);

            result.IsError.Should().BeFalse();
            result.Jwt.Should().NotBeNullOrEmpty();
            result.Client.ClientId.Should().Be("roclient");

            result.Claims.Count().Should().Be(8);
            var scopes = result.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToArray();
            scopes.Count().Should().Be(2);
            scopes[0].Should().Be("read");
            scopes[1].Should().Be("write");
        }
        
        [Fact]
        [Trait("Category", Category)]
        public async Task JWT_Token_invalid_Issuer()
        {
            var signer = Factory.CreateDefaultTokenCreator();
            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "roclient" }, "valid", 600, "read", "write");
            token.Issuer = "invalid";
            var jwt = await signer.CreateTokenAsync(token);

            var validator = Factory.CreateTokenValidator(null);
            var result = await validator.ValidateAccessTokenAsync(jwt);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task JWT_Token_Too_Long()
        {
            var signer = Factory.CreateDefaultTokenCreator();
            var jwt = await signer.CreateTokenAsync(TokenFactory.CreateAccessTokenLong(new Client { ClientId = "roclient" }, "valid", 600, 1000, "read", "write"));
            
            var validator = Factory.CreateTokenValidator(null);
            var result = await validator.ValidateAccessTokenAsync(jwt);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_AccessToken_but_Client_not_active()
        {
            var store = Factory.CreateReferenceTokenStore();
            var validator = Factory.CreateTokenValidator(store);

            var token = TokenFactory.CreateAccessToken(new Client { ClientId = "unknown" }, "valid", 600, "read", "write");

            var handle = await store.StoreReferenceTokenAsync(token);

            var result = await validator.ValidateAccessTokenAsync(handle);

            result.IsError.Should().BeTrue();
        }
    }
}
