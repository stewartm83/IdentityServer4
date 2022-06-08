// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation.AuthorizeRequest_Validation
{
    public class Authorize_ProtocolValidation_CustomValidator
    {
        private const string Category = "AuthorizeRequest Protocol Validation";

        private StubAuthorizeRequestValidator _stubAuthorizeRequestValidator = new StubAuthorizeRequestValidator();
        private AuthorizeRequestValidator _subject;

        public Authorize_ProtocolValidation_CustomValidator()
        {
            _subject = Factory.CreateAuthorizeRequestValidator(customValidator: _stubAuthorizeRequestValidator);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task should_call_custom_validator()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var result = await _subject.ValidateAsync(parameters);

            _stubAuthorizeRequestValidator.WasCalled.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task should_return_error_info_from_custom_validator()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            _stubAuthorizeRequestValidator.Callback = ctx =>
            {
                ctx.Result = new AuthorizeRequestValidationResult(ctx.Result.ValidatedRequest, "foo", "bar");
            };
            var result = await _subject.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("foo");
            result.ErrorDescription.Should().Be("bar");
        }
    }

    public class StubAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        public Action<CustomAuthorizeRequestValidationContext> Callback;
        public bool WasCalled { get; set; }

        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            WasCalled = true;
            Callback?.Invoke(context);
            return Task.CompletedTask;
        }
    }
}
