// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class BearerTokenUsageValidation
    {
        private const string Category = "BearerTokenUsageValidator Tests";

        [Fact]
        [Trait("Category", Category)]
        public async Task No_Header_no_Body_Get()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";

            var validator = new BearerTokenUsageValidator(TestLogger.Create< BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task No_Header_no_Body_Post()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Non_Bearer_Scheme_Header()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            ctx.Request.Headers.Add("Authorization", new string[] { "Foo Bar" });

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Bearer_Scheme_Header()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            ctx.Request.Headers.Add("Authorization", new string[] { "Bearer" });

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Whitespaces_Bearer_Scheme_Header()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            ctx.Request.Headers.Add("Authorization", new string[] { "Bearer           " });

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Bearer_Scheme_Header()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";
            ctx.Request.Headers.Add("Authorization", new string[] { "Bearer token" });

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeTrue();
            result.Token.Should().Be("token");
            result.UsageType.Should().Be(BearerTokenUsageType.AuthorizationHeader);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_Body_Post()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var body = "access_token=token";
            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeTrue();
            result.Token.Should().Be("token");
            result.UsageType.Should().Be(BearerTokenUsageType.PostBody);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Body_Post_empty_Token()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var body = "access_token=";
            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Body_Post_Whitespace_Token()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var body = "access_token=                ";
            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Body_Post_no_Token()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var body = "foo=bar";
            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var validator = new BearerTokenUsageValidator(TestLogger.Create<BearerTokenUsageValidator>());
            var result = await validator.ValidateAsync(ctx);

            result.TokenFound.Should().BeFalse();
        }
    }
}
