// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.Authorize
{
    public class AuthorizeEndpointTests
    {
        private const string Category = "Authorize Endpoint";

        private HttpContext _context;

        private TestEventService _fakeEventService = new TestEventService();

        private ILogger<AuthorizeEndpoint> _fakeLogger = TestLogger.Create<AuthorizeEndpoint>();

        private IdentityServerOptions _options = new IdentityServerOptions();

        private MockUserSession _mockUserSession = new MockUserSession();

        private NameValueCollection _params = new NameValueCollection();

        private StubAuthorizeRequestValidator _stubAuthorizeRequestValidator = new StubAuthorizeRequestValidator();

        private StubAuthorizeResponseGenerator _stubAuthorizeResponseGenerator = new StubAuthorizeResponseGenerator();

        private StubAuthorizeInteractionResponseGenerator _stubInteractionGenerator = new StubAuthorizeInteractionResponseGenerator();

        private AuthorizeEndpoint _subject;

        private ClaimsPrincipal _user = new IdentityServerUser("bob").CreatePrincipal();

        private ValidatedAuthorizeRequest _validatedAuthorizeRequest;

        public AuthorizeEndpointTests()
        {
            Init();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_authorize_path_should_return_authorization_result()
        {
            _context.Request.Method = "GET";
            _context.Request.Path = new PathString("/connect/authorize");
            _mockUserSession.User = _user;

            var result = await _subject.ProcessAsync(_context);

            result.Should().BeOfType<AuthorizeResult>();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_post_without_form_content_type_should_return_415()
        {
            _context.Request.Method = "POST";

            var result = await _subject.ProcessAsync(_context);

            var statusCode = result as StatusCodeResult;
            statusCode.Should().NotBeNull();
            statusCode.StatusCode.Should().Be(415);
        }

        internal void Init()
        {
            _context = new MockHttpContextAccessor().HttpContext;

            _validatedAuthorizeRequest = new ValidatedAuthorizeRequest()
            {
                RedirectUri = "http://client/callback",
                State = "123",
                ResponseMode = "fragment",
                ClientId = "client",
                Client = new Client
                {
                    ClientId = "client",
                    ClientName = "Test Client"
                },
                Raw = _params,
                Subject = _user
            };
            _stubAuthorizeResponseGenerator.Response.Request = _validatedAuthorizeRequest;

            _stubAuthorizeRequestValidator.Result = new AuthorizeRequestValidationResult(_validatedAuthorizeRequest);

            _subject = new AuthorizeEndpoint(
                _fakeEventService,
                _fakeLogger,
                _options,
                _stubAuthorizeRequestValidator,
                _stubInteractionGenerator,
                _stubAuthorizeResponseGenerator,
                _mockUserSession);
        }
    }
}
