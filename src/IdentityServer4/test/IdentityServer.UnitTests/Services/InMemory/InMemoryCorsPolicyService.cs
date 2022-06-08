// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Xunit;

namespace IdentityServer.UnitTests.Services.InMemory
{
    public class InMemoryCorsPolicyServiceTests
    {
        private const string Category = "InMemoryCorsPolicyService";

        private InMemoryCorsPolicyService _subject;
        private List<Client> _clients = new List<Client>();

        public InMemoryCorsPolicyServiceTests()
        {
            _subject = new InMemoryCorsPolicyService(TestLogger.Create<InMemoryCorsPolicyService>(), _clients);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_origin_should_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo"
                }
            });

            _subject.IsOriginAllowedAsync("http://foo").Result.Should().BeTrue();
        }

        [Theory]
        [InlineData("http://foo")]
        [InlineData("https://bar")]
        [InlineData("http://bar-baz")]
        [Trait("Category", Category)]
        public void client_does_not_has_origin_should_not_allow_origin(string clientOrigin)
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    clientOrigin
                }
            });
            _subject.IsOriginAllowedAsync("http://bar").Result.Should().Be(false);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_many_origins_and_origin_is_in_list_should_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo",
                    "http://bar",
                    "http://baz"
                }
            });
            _subject.IsOriginAllowedAsync("http://bar").Result.Should().Be(true);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_many_origins_and_origin_is_in_not_list_should_not_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo",
                    "http://bar",
                    "http://baz"
                }
            });
            _subject.IsOriginAllowedAsync("http://quux").Result.Should().Be(false);
        }

        [Fact]
        [Trait("Category", Category)]
        public void many_clients_have_same_origins_should_allow_origin()
        {
            _clients.AddRange(new Client[] {
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo"
                    }
                },
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo"
                    }
                }
            });
            _subject.IsOriginAllowedAsync("http://foo").Result.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public void handle_invalid_cors_origin_format_exception()
        {
            _clients.AddRange(new Client[] {
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo",
                        "http://ba z"
                    }
                },
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo",
                        "http://bar"
                    }
                }
            });
            _subject.IsOriginAllowedAsync("http://bar").Result.Should().BeTrue();
        }
    }
}
