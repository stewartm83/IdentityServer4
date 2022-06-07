// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class DynamicParameterExtensionGrantValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var impersonatedClient = context.Request.Raw.Get("impersonated_client");
            var lifetime = context.Request.Raw.Get("lifetime");
            var extraClaim = context.Request.Raw.Get("claim");
            var tokenType = context.Request.Raw.Get("type");
            var sub = context.Request.Raw.Get("sub");

            if (!string.IsNullOrEmpty(impersonatedClient))
            {
                context.Request.ClientId = impersonatedClient;
            }

            if (!string.IsNullOrEmpty(lifetime))
            {
                context.Request.AccessTokenLifetime = int.Parse(lifetime);
            }

            if (!string.IsNullOrEmpty(tokenType))
            {
                if (tokenType == "jwt")
                {
                    context.Request.AccessTokenType = AccessTokenType.Jwt;
                }
                else if (tokenType == "reference")
                {
                    context.Request.AccessTokenType = AccessTokenType.Reference;
                }
            }

            if (!string.IsNullOrEmpty(extraClaim))
            {
                context.Request.ClientClaims.Add(new Claim("extra", extraClaim));
            }

            if (!string.IsNullOrEmpty(sub))
            {
                context.Result = new GrantValidationResult(sub, "delegation");
            }
            else
            {
                context.Result = new GrantValidationResult();
            }

            return Task.CompletedTask;
        }

        public string GrantType => "dynamic";
    }
}
