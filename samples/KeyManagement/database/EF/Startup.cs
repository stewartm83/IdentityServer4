// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.KeyManagement.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace sample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public ILoggerFactory LoggerFactory { get; set; }

        public Startup(IConfiguration config, IWebHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            Configuration = config;
            Environment = environment;
            LoggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //var name = "CN=test.dataprotection";
            //var cert = X509.LocalMachine.My.SubjectDistinguishedName.Find(name, false).FirstOrDefault();

            var cn = Configuration.GetConnectionString("db");

            services.AddDataProtection()
                .PersistKeysToDatabase(new DatabaseKeyManagementOptions
                {
                    ConfigureDbContext = b => b.UseSqlServer(cn),
                    LoggerFactory = LoggerFactory,
                });
                //.ProtectKeysWithCertificate(cert);

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddSigningKeyManagement(
                    options => // configuring options is optional :)
                    {
                        options.DeleteRetiredKeys = true;
                        options.KeyType = IdentityServer4.KeyManagement.KeyType.RSA;

                        // all of these values in here are changed for local testing
                        options.InitializationDuration = TimeSpan.FromSeconds(5);
                        options.InitializationSynchronizationDelay = TimeSpan.FromSeconds(1);

                        options.KeyActivationDelay = TimeSpan.FromSeconds(10);
                        options.KeyExpiration = options.KeyActivationDelay * 2;
                        options.KeyRetirement = options.KeyActivationDelay * 3;

                        // You can get your own license from:
                        // https://www.identityserver.com/products/KeyManagement
                        options.Licensee = "your licensee";
                        options.License = "your license key";
                    })
                    .PersistKeysToDatabase(new DatabaseKeyManagementOptions {
                        ConfigureDbContext = b => b.UseSqlServer(cn),
                    })
                    .ProtectKeysWithDataProtection()
                    //.EnableInMemoryCaching() // caching disabled unless explicitly enabled
                ;
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
