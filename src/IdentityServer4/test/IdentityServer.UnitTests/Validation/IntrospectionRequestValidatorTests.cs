// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class IntrospectionRequestValidatorTests
    {
        private const string Category = "Introspection request validation";

        private IntrospectionRequestValidator _subject;
        private IReferenceTokenStore _referenceTokenStore;

        public IntrospectionRequestValidatorTests()
        {
            _referenceTokenStore = Factory.CreateReferenceTokenStore();
            var tokenValidator = Factory.CreateTokenValidator(_referenceTokenStore);

            _subject = new IntrospectionRequestValidator(tokenValidator, TestLogger.Create<IntrospectionRequestValidator>());
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task valid_token_should_successfully_validate()
        {
            var token = new Token {
                CreationTime = DateTime.UtcNow,
                Issuer = "http://op",
                ClientId = "codeclient",
                Lifetime = 1000,
                Claims =
                {
                    new System.Security.Claims.Claim("scope", "a"),
                    new System.Security.Claims.Claim("scope", "b")
                }
            };
            var handle = await _referenceTokenStore.StoreReferenceTokenAsync(token);
            
            var param = new NameValueCollection()
            {
                { "token", handle}
            };

            var result = await _subject.ValidateAsync(param, null);

            result.IsError.Should().Be(false);
            result.IsActive.Should().Be(true);
            result.Claims.Count().Should().Be(5);
            result.Token.Should().Be(handle);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task missing_token_should_error()
        {
            var param = new NameValueCollection();
            
            var result = await _subject.ValidateAsync(param, null);

            result.IsError.Should().Be(true);
            result.Error.Should().Be("missing_token");
            result.IsActive.Should().Be(false);
            result.Claims.Should().BeNull();
            result.Token.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task invalid_token_should_return_inactive()
        {
            var param = new NameValueCollection()
            {
                { "token", "invalid" }
            };

            var result = await _subject.ValidateAsync(param, null);

            result.IsError.Should().Be(false);
            result.IsActive.Should().Be(false);
            result.Claims.Should().BeNull();
            result.Token.Should().Be("invalid");
        }
    }
}
