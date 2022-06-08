// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.DbContexts
{
    public class ClientDbContextTests : IntegrationTest<ClientDbContextTests, ConfigurationDbContext, ConfigurationStoreOptions>
    {
        public ClientDbContextTests(DatabaseProviderFixture<ConfigurationDbContext> fixture) : base(fixture)
        {
            foreach (var options in TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ConfigurationDbContext>)y)).ToList())
            {
                using (var context = new ConfigurationDbContext(options, StoreOptions))
                    context.Database.EnsureCreated();
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void CanAddAndDeleteClientScopes(DbContextOptions<ConfigurationDbContext> options)
        {
            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client-scopes",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                // explicit include due to lack of EF Core lazy loading
                var client = db.Clients.Include(x => x.AllowedScopes).First();

                client.AllowedScopes.Add(new ClientScope
                {
                    Scope = "test"
                });

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                var client = db.Clients.Include(x => x.AllowedScopes).First();
                var scope = client.AllowedScopes.First();

                client.AllowedScopes.Remove(scope);

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                var client = db.Clients.Include(x => x.AllowedScopes).First();

                Assert.Empty(client.AllowedScopes);
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void CanAddAndDeleteClientRedirectUri(DbContextOptions<ConfigurationDbContext> options)
        {
            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();

                client.RedirectUris.Add(new ClientRedirectUri
                {
                    RedirectUri = "https://redirect-uri-1"
                });

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();
                var redirectUri = client.RedirectUris.First();

                client.RedirectUris.Remove(redirectUri);

                db.SaveChanges();
            }

            using (var db = new ConfigurationDbContext(options, StoreOptions))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();

                Assert.Empty(client.RedirectUris);
            }
        }
    }
}
