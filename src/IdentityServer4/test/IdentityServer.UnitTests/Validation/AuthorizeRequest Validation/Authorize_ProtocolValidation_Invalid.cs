﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using Xunit;

namespace IdentityServer.UnitTests.Validation.AuthorizeRequest_Validation
{
    public class Authorize_ProtocolValidation_Invalid
    {
        private const string Category = "AuthorizeRequest Protocol Validation";

        [Fact]
        [Trait("Category", Category)]
        public async Task Null_Parameter()
        {
            var validator = Factory.CreateAuthorizeRequestValidator();

            Func<Task> act = () => validator.ValidateAsync(null);

           await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Parameters()
        {
            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(new NameValueCollection());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        // fails because openid scope is requested, but no response type that indicates an identity token
        [Fact]
        [Trait("Category", Category)]
        public async Task OpenId_Token_Only_Request()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, IdentityServerConstants.StandardScopes.OpenId);
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Token);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Resource_Only_IdToken_Request()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "resource");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.IdToken);
            parameters.Add(OidcConstants.AuthorizeRequest.Nonce, "abc");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Mixed_Token_Only_Request()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid resource");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Token);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidScope);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task OpenId_IdToken_Request_Nonce_Missing()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.IdToken);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ClientId()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/callback");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Scope()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_RedirectUri()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "client");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Malformed_RedirectUri()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "client");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "malformed");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Malformed_RedirectUri_Triple_Slash()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "client");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https:///attacker.com");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.UnsupportedResponseType);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, "unknown");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.UnsupportedResponseType);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_IdToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.IdToken);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_IdTokenToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.IdTokenToken);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_CodeToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "hybridclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.CodeToken);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_CodeIdToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "hybridclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.CodeIdToken);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_CodeIdTokenToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "hybridclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.CodeIdTokenToken);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Malformed_MaxAge()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);
            parameters.Add(OidcConstants.AuthorizeRequest.MaxAge, "malformed");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Negative_MaxAge()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);
            parameters.Add(OidcConstants.AuthorizeRequest.MaxAge, "-1");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_TokenIdToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "implicitclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "oob://implicit/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, "token id_token"); // Unconventional order
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_ResponseMode_For_IdTokenCodeToken_ResponseType()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "hybridclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, "id_token code token"); // Unconventional ordering
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Query);

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task prompt_none_and_other_values_should_fail()
        {
            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, "codeclient");
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, "openid");
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, "https://server/cb");
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code);
            parameters.Add(OidcConstants.AuthorizeRequest.ResponseMode, OidcConstants.ResponseModes.Fragment);
            parameters.Add(OidcConstants.AuthorizeRequest.Prompt, "none login");

            var validator = Factory.CreateAuthorizeRequestValidator();
            var result = await validator.ValidateAsync(parameters);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.AuthorizeErrors.InvalidRequest);
        }
    }
}
