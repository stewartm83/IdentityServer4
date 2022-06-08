// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class Startup
    {
        static public ICustomTokenRequestValidator CustomTokenRequestValidator { get; set; } 

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();

            var builder = services.AddIdentityServer(options =>
            {
                options.IssuerUri = "https://idsvr4";

                options.Events = new EventsOptions
                {
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true,
                    RaiseSuccessEvents = true
                };
            });

            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryIdentityResources(Scopes.GetIdentityScopes());
            builder.AddInMemoryApiResources(Scopes.GetApiResources());
            builder.AddInMemoryApiScopes(Scopes.GetApiScopes());
            builder.AddTestUsers(Users.Get());

            builder.AddDeveloperSigningCredential(persistKey: false);

            builder.AddExtensionGrantValidator<ExtensionGrantValidator>();
            builder.AddExtensionGrantValidator<ExtensionGrantValidator2>();
            builder.AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>();
            builder.AddExtensionGrantValidator<DynamicParameterExtensionGrantValidator>();

            builder.AddProfileService<CustomProfileService>();

            builder.AddJwtBearerClientAuthentication();
            builder.AddSecretValidator<ConfirmationSecretValidator>();

            // add a custom token request validator if set
            if (CustomTokenRequestValidator != null)
            {
                builder.Services.AddTransient(r => CustomTokenRequestValidator);
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}
