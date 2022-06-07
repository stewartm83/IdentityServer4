// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer.UnitTests.Services.Default
{
    public class DefaultTokenServiceTests
    {
        private DefaultTokenService _subject;

        MockClaimsService _mockClaimsService = new MockClaimsService();
        MockReferenceTokenStore _mockReferenceTokenStore = new MockReferenceTokenStore();
        MockTokenCreationService _mockTokenCreationService = new MockTokenCreationService();
        DefaultHttpContext _httpContext = new DefaultHttpContext();
        MockSystemClock _mockSystemClock = new MockSystemClock();
        MockKeyMaterialService _mockKeyMaterialService = new MockKeyMaterialService();
        IdentityServerOptions _options = new IdentityServerOptions();

        public DefaultTokenServiceTests()
        {
            _options.IssuerUri = "https://test.identityserver.io";

            var svcs = new ServiceCollection();
            svcs.AddSingleton(_options);
            _httpContext.RequestServices = svcs.BuildServiceProvider();

            _subject = new DefaultTokenService(
                _mockClaimsService,
                _mockReferenceTokenStore,
                _mockTokenCreationService,
                new HttpContextAccessor { HttpContext = _httpContext },
                _mockSystemClock,
                _mockKeyMaterialService,
                _options,
                TestLogger.Create<DefaultTokenService>());
        }

        [Fact]
        public async Task CreateAccessTokenAsync_should_include_aud_for_each_ApiResource()
        {
            var request = new TokenCreationRequest { 
                ValidatedResources = new ResourceValidationResult()
                {
                    Resources = new Resources()
                    {
                        ApiResources = 
                        {
                            new ApiResource("api1"){ Scopes = { "scope1" } },
                            new ApiResource("api2"){ Scopes = { "scope2" } },
                            new ApiResource("api3"){ Scopes = { "scope3" } },
                        },
                    },
                    ParsedScopes =
                    {
                        new ParsedScopeValue("scope1"),
                        new ParsedScopeValue("scope2"),
                        new ParsedScopeValue("scope3"),
                    }
                },
                ValidatedRequest = new ValidatedRequest()
                {
                    Client = new Client { }
                }
            };

            var result = await _subject.CreateAccessTokenAsync(request);

            result.Audiences.Count.Should().Be(3);
            result.Audiences.Should().BeEquivalentTo(new[] { "api1", "api2", "api3" });
        }

        [Fact]
        public async Task CreateAccessTokenAsync_when_no_apiresources_should_not_include_any_aud()
        {
            var request = new TokenCreationRequest
            {
                ValidatedResources = new ResourceValidationResult()
                {
                    Resources = new Resources()
                    {
                        ApiScopes =
                        {
                            new ApiScope("scope1"),
                            new ApiScope("scope2"),
                            new ApiScope("scope3"),
                        },
                    },
                    ParsedScopes =
                    {
                        new ParsedScopeValue("scope1"),
                        new ParsedScopeValue("scope2"),
                        new ParsedScopeValue("scope3"),
                    }
                },
                ValidatedRequest = new ValidatedRequest()
                {
                    Client = new Client { }
                }
            };

            var result = await _subject.CreateAccessTokenAsync(request);

            result.Audiences.Count.Should().Be(0);
        }


        [Fact]
        public async Task CreateAccessTokenAsync_when_no_session_should_not_include_sid()
        {
            var request = new TokenCreationRequest
            {
                ValidatedResources = new ResourceValidationResult(),
                ValidatedRequest = new ValidatedRequest()
                {
                    Client = new Client { },
                    SessionId = null
                }
            };

            var result = await _subject.CreateAccessTokenAsync(request);

            result.Claims.SingleOrDefault(x => x.Type == JwtClaimTypes.SessionId).Should().BeNull();
        }
        [Fact]
        public async Task CreateAccessTokenAsync_when_session_should_include_sid()
        {
            var request = new TokenCreationRequest
            {
                ValidatedResources = new ResourceValidationResult(),
                ValidatedRequest = new ValidatedRequest()
                {
                    Client = new Client { },
                    SessionId = "123"
                }
            };

            var result = await _subject.CreateAccessTokenAsync(request);

            result.Claims.SingleOrDefault(x => x.Type == JwtClaimTypes.SessionId).Value.Should().Be("123");
        }
    }
}
