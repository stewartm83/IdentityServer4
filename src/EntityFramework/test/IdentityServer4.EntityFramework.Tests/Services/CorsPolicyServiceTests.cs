// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Services;

namespace IdentityServer4.EntityFramework.IntegrationTests.Services
{
    public class CorsPolicyServiceTests : IntegrationTest<CorsPolicyServiceTests, ConfigurationDbContext, ConfigurationStoreOptions>
    {
        public CorsPolicyServiceTests(DatabaseProviderFixture<ConfigurationDbContext> fixture) : base(fixture)
        {
            foreach (var options in TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ConfigurationDbContext>)y)).ToList())
            {
                using (var context = new ConfigurationDbContext(options, StoreOptions))
                    context.Database.EnsureCreated();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue(DbContextOptions<ConfigurationDbContext> options)
        {
            const string testCorsOrigin = "https://identityserver.io/";

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.Clients.Add(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }.ToEntity());
                context.Clients.Add(new Client
                {
                    ClientId = "2",
                    ClientName = "2",
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com", testCorsOrigin }
                }.ToEntity());
                context.SaveChanges();
            }

            bool result;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var ctx = new DefaultHttpContext();
                var svcs = new ServiceCollection();
                svcs.AddSingleton<IConfigurationDbContext>(context);
                ctx.RequestServices = svcs.BuildServiceProvider();
                var ctxAccessor = new HttpContextAccessor();
                ctxAccessor.HttpContext = ctx;

                var service = new CorsPolicyService(ctxAccessor, FakeLogger<CorsPolicyService>.Create());
                result = service.IsOriginAllowedAsync(testCorsOrigin).Result;
            }

            Assert.True(result);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse(DbContextOptions<ConfigurationDbContext> options)
        {
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.Clients.Add(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }.ToEntity());
                context.SaveChanges();
            }

            bool result;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var ctx = new DefaultHttpContext();
                var svcs = new ServiceCollection();
                svcs.AddSingleton<IConfigurationDbContext>(context);
                ctx.RequestServices = svcs.BuildServiceProvider();
                var ctxAccessor = new HttpContextAccessor();
                ctxAccessor.HttpContext = ctx;

                var service = new CorsPolicyService(ctxAccessor, FakeLogger<CorsPolicyService>.Create());
                result = service.IsOriginAllowedAsync("InvalidOrigin").Result;
            }

            Assert.False(result);
        }
    }
}
