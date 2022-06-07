// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Validation.Setup;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class ClientSecretValidation
    {
        private const string Category = "Secrets - Client Secret Validator";

        [Fact]
        [Trait("Category", Category)]
        public async Task confidential_client_with_correct_secret_should_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient&client_secret=secret";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeFalse();
            result.Client.ClientId.Should().Be("roclient");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task confidential_client_with_incorrect_secret_should_not_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient&client_secret=invalid";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task public_client_without_secret_should_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient.public";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeFalse();
            result.Client.ClientId.Should().Be("roclient.public");
            result.Client.RequireClientSecret.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task implicit_client_without_secret_should_be_able_to_authenticate()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=client.implicit";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeFalse();
            result.Client.ClientId.Should().Be("client.implicit");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task implicit_client_and_client_creds_without_secret_should_not_be_able_to_authenticate()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=implicit_and_client_creds";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeTrue();
        }
    }
}
