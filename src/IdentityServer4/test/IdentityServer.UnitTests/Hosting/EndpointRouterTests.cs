// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;
using static IdentityServer4.Constants;

namespace IdentityServer.UnitTests.Hosting
{
    public class EndpointRouterTests
    {
        private Dictionary<string, IdentityServer4.Hosting.Endpoint> _pathMap;
        private List<IdentityServer4.Hosting.Endpoint> _endpoints;
        private IdentityServerOptions _options;
        private EndpointRouter _subject;

        public EndpointRouterTests()
        {
            _pathMap = new Dictionary<string, IdentityServer4.Hosting.Endpoint>();
            _endpoints = new List<IdentityServer4.Hosting.Endpoint>();
            _options = new IdentityServerOptions();
            _subject = new EndpointRouter(_endpoints, _options, TestLogger.Create<EndpointRouter>());
        }

        [Fact]
        public void Endpoint_ctor_requires_path_to_start_with_slash()
        {
            Action a = () => new IdentityServer4.Hosting.Endpoint("ep1", "ep1", typeof(MyEndpointHandler));
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Find_should_return_null_for_incorrect_path()
        {
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep1", "/ep1", typeof(MyEndpointHandler)));
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep2", "/ep2", typeof(MyOtherEndpointHandler)));

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = new PathString("/wrong");
            ctx.RequestServices = new StubServiceProvider();

            var result = _subject.Find(ctx);
            result.Should().BeNull();
        }

        [Fact]
        public void Find_should_find_path()
        {
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep1", "/ep1", typeof(MyEndpointHandler)));
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep2", "/ep2", typeof(MyOtherEndpointHandler)));

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = new PathString("/ep1");
            ctx.RequestServices = new StubServiceProvider();

            var result = _subject.Find(ctx);
            result.Should().BeOfType<MyEndpointHandler>();
        }

        [Fact]
        public void Find_should_not_find_nested_paths()
        {
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep1", "/ep1", typeof(MyEndpointHandler)));
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep2", "/ep2", typeof(MyOtherEndpointHandler)));

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = new PathString("/ep1/subpath");
            ctx.RequestServices = new StubServiceProvider();

            var result = _subject.Find(ctx);
            result.Should().BeNull();
        }

        [Fact]
        public void Find_should_find_first_registered_mapping()
        {
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep1", "/ep1", typeof(MyEndpointHandler)));
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep1", "/ep1", typeof(MyOtherEndpointHandler)));

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = new PathString("/ep1");
            ctx.RequestServices = new StubServiceProvider();

            var result = _subject.Find(ctx);
            result.Should().BeOfType<MyEndpointHandler>();
        }

        [Fact]
        public void Find_should_return_null_for_disabled_endpoint()
        {
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint(EndpointNames.Authorize, "/ep1", typeof(MyEndpointHandler)));
            _endpoints.Add(new IdentityServer4.Hosting.Endpoint("ep2", "/ep2", typeof(MyOtherEndpointHandler)));

            _options.Endpoints.EnableAuthorizeEndpoint = false;

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = new PathString("/ep1");
            ctx.RequestServices = new StubServiceProvider();

            var result = _subject.Find(ctx);
            result.Should().BeNull();
        }

        private class MyEndpointHandler : IEndpointHandler
        {
            public Task<IEndpointResult> ProcessAsync(HttpContext context)
            {
                throw new NotImplementedException();
            }
        }

        private class MyOtherEndpointHandler : IEndpointHandler
        {
            public Task<IEndpointResult> ProcessAsync(HttpContext context)
            {
                throw new NotImplementedException();
            }
        }

        private class StubServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(MyEndpointHandler)) return new MyEndpointHandler();
                if (serviceType == typeof(MyOtherEndpointHandler)) return new MyOtherEndpointHandler();

                throw new InvalidOperationException();
            }
        }
    }
}
