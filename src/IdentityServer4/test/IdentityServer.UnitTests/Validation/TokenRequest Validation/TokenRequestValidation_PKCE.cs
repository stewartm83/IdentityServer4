// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer.UnitTests.Validation.TokenRequest_Validation
{
    public class TokenRequestValidation_PKCE
    {
        private const string Category = "TokenRequest Validation - PKCE";

        private IClientStore _clients = Factory.CreateClientStore();
        private InputLengthRestrictions lengths = new InputLengthRestrictions();

        [Theory]
        [InlineData("codeclient.pkce")]
        [InlineData("codeclient")]
        [Trait("Category", Category)]
        public async Task valid_pkce_token_request_with_plain_method_should_succeed(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            var grants = Factory.CreateAuthorizationCodeStore();
            var verifier = "x".Repeat(lengths.CodeVerifierMinLength);

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("bob").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                CodeChallenge = verifier.Sha256(),
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Plain,

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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, verifier);
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task valid_pkce_token_request_with_plain_method_should_succeed_hybrid()
        {
            var client = await _clients.FindEnabledClientByIdAsync("hybridclient.pkce");
            var grants = Factory.CreateAuthorizationCodeStore();
            var verifier = "x".Repeat(lengths.CodeVerifierMinLength);

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                CodeChallenge = verifier.Sha256(),
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Plain,

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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, verifier);
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("codeclient.pkce")]
        [InlineData("codeclient")]
        [Trait("Category", Category)]
        public async Task valid_pkce_token_request_with_sha256_method_should_succeed(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            var grants = Factory.CreateAuthorizationCodeStore();

            var verifier = "x".Repeat(lengths.CodeVerifierMinLength);
            var challenge = VerifierToSha256CodeChallenge(verifier);
            
            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                CodeChallenge = challenge.Sha256(),
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256,

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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, verifier);
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("codeclient.pkce")]
        [Trait("Category", Category)]
        public async Task token_request_with_missing_code_challenge_and_verifier_should_fail(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
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

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Theory]
        [InlineData("codeclient.pkce")]
        [InlineData("codeclient")]
        [Trait("Category", Category)]
        public async Task token_request_with_missing_code_challenge_should_fail(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, "x".Repeat(lengths.CodeVerifierMinLength));
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Theory]
        [InlineData("codeclient.pkce")]
        [InlineData("codeclient")]
        [Trait("Category", Category)]
        public async Task token_request_with_invalid_verifier_plain_method_should_fail(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            var grants = Factory.CreateAuthorizationCodeStore();
            var verifier = "x".Repeat(lengths.CodeVerifierMinLength);

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                CodeChallenge = verifier.Sha256(),
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Plain,

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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, verifier + "invalid");
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Theory]
        [InlineData("codeclient.pkce")]
        [InlineData("codeclient")]
        [Trait("Category", Category)]
        public async Task token_request_with_invalid_verifier_sha256_method_should_fail(string clientId)
        {
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            var grants = Factory.CreateAuthorizationCodeStore();

            var verifier = "x".Repeat(lengths.CodeVerifierMinLength);
            var challenge = VerifierToSha256CodeChallenge(verifier);

            var code = new AuthorizationCode
            {
                CreationTime = DateTime.UtcNow,
                Subject = new IdentityServerUser("123").CreatePrincipal(),
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                RedirectUri = "https://server/cb",
                CodeChallenge = challenge.Sha256(),
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256,

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
            parameters.Add(OidcConstants.TokenRequest.CodeVerifier, verifier + "invalid");
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client.ToValidationResult());

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        private static string VerifierToSha256CodeChallenge(string codeVerifier)
        {
            var codeVerifierBytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hashedBytes = codeVerifierBytes.Sha256();
            var transformedCodeVerifier = Base64Url.Encode(hashedBytes);

            return transformedCodeVerifier;
        }
    }
}
