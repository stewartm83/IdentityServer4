// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using FluentAssertions;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void GetCorsOrigin_valid_cors_request_should_return_cors_origin()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Scheme = "http";
            ctx.Request.Host = new HostString("foo");
            ctx.Request.Headers.Add("Origin", "http://bar");

            ctx.Request.GetCorsOrigin().Should().Be("http://bar");
        }

        [Fact]
        public void GetCorsOrigin_origin_from_same_host_should_not_return_cors_origin()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Scheme = "http";
            ctx.Request.Host = new HostString("foo");
            ctx.Request.Headers.Add("Origin", "http://foo");

            ctx.Request.GetCorsOrigin().Should().BeNull();
        }

        [Fact]
        public void GetCorsOrigin_no_origin_should_not_return_cors_origin()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Scheme = "http";
            ctx.Request.Host = new HostString("foo");

            ctx.Request.GetCorsOrigin().Should().BeNull();
        }
    }
}
