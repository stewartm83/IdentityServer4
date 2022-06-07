// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer.IntegrationTests.Clients.Setup;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class DiscoveryClientTests
    {
        private const string DiscoveryEndpoint = "https://server/.well-known/openid-configuration";

        private readonly HttpClient _client;

        public DiscoveryClientTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _client = server.CreateClient();
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_values()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy =
                {
                    ValidateIssuerName = false
                }
            });

            // endpoints
            doc.TokenEndpoint.Should().Be("https://server/connect/token");
            doc.AuthorizeEndpoint.Should().Be("https://server/connect/authorize");
            doc.IntrospectionEndpoint.Should().Be("https://server/connect/introspect");
            doc.EndSessionEndpoint.Should().Be("https://server/connect/endsession");

            // jwk
            doc.KeySet.Keys.Count.Should().Be(1);
            doc.KeySet.Keys.First().E.Should().NotBeNull();
            doc.KeySet.Keys.First().N.Should().NotBeNull();
        }
    }
}
