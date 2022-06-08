// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.EndSession
{
    public class EndSessionCallbackResultTests
    {
        private const string Category = "End Session Callback Result";

        private readonly EndSessionCallbackValidationResult _validationResult;
        private readonly IdentityServerOptions _options;
        private readonly EndSessionCallbackResult _subject;

        public EndSessionCallbackResultTests()
        {
            _validationResult = new EndSessionCallbackValidationResult()
            {
                IsError = false,
            };
            _options = new IdentityServerOptions();
            _subject = new EndSessionCallbackResult(_validationResult, _options);
        }

        [Fact]
        public async Task default_options_should_emit_frame_src_csp_headers()
        {
            _validationResult.FrontChannelLogoutUrls = new[] { "http://foo" };

            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";

            await _subject.ExecuteAsync(ctx);

            ctx.Response.Headers["Content-Security-Policy"].First().Should().Contain("frame-src http://foo");
        }

        [Fact]
        public async Task relax_csp_options_should_prevent_frame_src_csp_headers()
        {
            _options.Authentication.RequireCspFrameSrcForSignout = false;
            _validationResult.FrontChannelLogoutUrls = new[] { "http://foo" };

            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "GET";

            await _subject.ExecuteAsync(ctx);

            ctx.Response.Headers["Content-Security-Policy"].FirstOrDefault().Should().BeNull();
        }
    }
}
