// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Text;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class BasicAuthenticationSecretParsing
    {
        private const string Category = "Secrets - Basic Authentication Secret Parsing";

        private IdentityServerOptions _options;
        private BasicAuthenticationSecretParser _parser;

        public BasicAuthenticationSecretParsing()
        {
            _options = new IdentityServerOptions();
            _parser = new BasicAuthenticationSecretParser(_options, TestLogger.Create<BasicAuthenticationSecretParser>());
        }

        [Fact]
        [Trait("Category", Category)]
        public async void EmptyContext()
        {
            var context = new DefaultHttpContext();

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void Valid_BasicAuthentication_Request()
        {
            var context = new DefaultHttpContext();

            var headerValue = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("client:secret")));
            context.Request.Headers.Add("Authorization", new StringValues(headerValue));


            var secret = await _parser.ParseAsync(context);

            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.SharedSecret);
            secret.Id.Should().Be("client");
            secret.Credential.Should().Be("secret");
        }

        [Fact]
        [Trait("Category", Category)]
        public async void Valid_BasicAuthentication_Request_With_UserName_Only_And_Colon_For_Optional_ClientSecret()
        {
            var context = new DefaultHttpContext();

            var headerValue = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("client:")));
            context.Request.Headers.Add("Authorization", new StringValues(headerValue));

            var secret = await _parser.ParseAsync(context);

            secret.Type.Should().Be(IdentityServerConstants.ParsedSecretTypes.NoSecret);
            secret.Id.Should().Be("client");
            secret.Credential.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void BasicAuthentication_Request_With_Empty_Basic_Header()
        {
            var context = new DefaultHttpContext();

            context.Request.Headers.Add("Authorization", new StringValues(string.Empty));

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void Valid_BasicAuthentication_Request_ClientId_Too_Long()
        {
            var context = new DefaultHttpContext();

            var longClientId = "x".Repeat(_options.InputLengthRestrictions.ClientId + 1);
            var credential = string.Format("{0}:secret", longClientId);

            var headerValue = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential)));
            context.Request.Headers.Add("Authorization", new StringValues(headerValue));

            var secret = await _parser.ParseAsync(context);
            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void Valid_BasicAuthentication_Request_ClientSecret_Too_Long()
        {
            var context = new DefaultHttpContext();

            var longClientSecret = "x".Repeat(_options.InputLengthRestrictions.ClientSecret + 1);
            var credential = string.Format("client:{0}", longClientSecret);

            var headerValue = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential)));
            context.Request.Headers.Add("Authorization", new StringValues(headerValue));

            var secret = await _parser.ParseAsync(context);
            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void BasicAuthentication_Request_With_Empty_Basic_Header_Variation()
        {
            var context = new DefaultHttpContext();

            context.Request.Headers.Add("Authorization", new StringValues("Basic "));

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void BasicAuthentication_Request_With_Unknown_Scheme()
        {
            var context = new DefaultHttpContext();

            context.Request.Headers.Add("Authorization", new StringValues("Unknown"));

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void BasicAuthentication_Request_With_Malformed_Credentials_NoBase64_Encoding()
        {
            var context = new DefaultHttpContext();

            context.Request.Headers.Add("Authorization", new StringValues("Basic somerandomdata"));

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }

        [Fact]
        [Trait("Category", Category)]
        public async void BasicAuthentication_Request_With_Malformed_Credentials_Base64_Encoding_UserName_Only()
        {
            var context = new DefaultHttpContext();

            var headerValue = string.Format("Basic {0}",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("client")));
            context.Request.Headers.Add("Authorization", new StringValues(headerValue));

            var secret = await _parser.ParseAsync(context);

            secret.Should().BeNull();
        }
    }
}
