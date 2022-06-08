// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.UnitTests.Common;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class UserInfoRequestValidation
    {
        private const string Category = "UserInfo Request Validation Tests";
        private IClientStore _clients = new InMemoryClientStore(TestClients.Get());

        [Fact]
        [Trait("Category", Category)]
        public async Task token_without_sub_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>()
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task active_user_should_succeed()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>
                {
                    new Claim("sub", "123")
                },
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: true),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task inactive_user_should_fail()
        {
            var tokenResult = new TokenValidationResult
            {
                IsError = false,
                Client = await _clients.FindEnabledClientByIdAsync("codeclient"),
                Claims = new List<Claim>
                {
                    new Claim("sub", "123")
                },
            };

            var validator = new UserInfoRequestValidator(
                new TestTokenValidator(tokenResult),
                new TestProfileService(shouldBeActive: false),
                TestLogger.Create<UserInfoRequestValidator>());

            var result = await validator.ValidateRequestAsync("token");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.ProtectedResourceErrors.InvalidToken);
        }
    }
}
