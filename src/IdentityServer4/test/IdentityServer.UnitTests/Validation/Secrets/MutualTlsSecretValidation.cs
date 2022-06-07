// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class MutualTlsSecretValidation
    {
        private const string Category = "Secrets - MutualTls Secret Validation";
       
        private IClientStore _clients = new InMemoryClientStore(ClientValidationTestClients.Get());

        ///////////////////
        // thumbprints
        ///////////////////

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_invalid_secret_type_should_not_match()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_missing_cert_should_throw()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            Func<Task> act = async () => await validator.ValidateAsync(client.ClientSecrets, secret);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_invalid_secret_should_not_match()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Thumbprint_valid_secret_should_match()
        {
            ISecretValidator validator = new X509ThumbprintSecretValidator(new Logger<X509ThumbprintSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_valid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeTrue();
        }

        ///////////////////
        // names
        ///////////////////

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_invalid_secret_type_should_not_match()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_missing_cert_should_throw()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = "secret",
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            Func<Task> act = async () => await validator.ValidateAsync(client.ClientSecrets, secret);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_invalid_secret_should_not_match()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_invalid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Name_valid_secret_should_match()
        {
            ISecretValidator validator = new X509NameSecretValidator(new Logger<X509NameSecretValidator>(new LoggerFactory()));

            var clientId = "mtls_client_valid";
            var client = await _clients.FindEnabledClientByIdAsync(clientId);

            var secret = new ParsedSecret
            {
                Id = clientId,
                Credential = TestCert.Load(),
                Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
            };

            var result = await validator.ValidateAsync(client.ClientSecrets, secret);

            result.Success.Should().BeTrue();
        }
    }
}
