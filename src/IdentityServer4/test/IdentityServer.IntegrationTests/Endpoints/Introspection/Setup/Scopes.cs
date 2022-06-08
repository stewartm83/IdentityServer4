// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    internal class Scopes
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource
                {
                    Name = "api1",
                    ApiSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes = { "api1" }
                },
                new ApiResource
                {
                    Name = "api2",
                    ApiSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes = { "api2" }
                },
                 new ApiResource
                {
                    Name = "api3",
                    ApiSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes = { "api3-a", "api3-b" }
                }
            };
        }
        public static IEnumerable<ApiScope> GetScopes()
        {
            return new ApiScope[]
            {
                new ApiScope
                {
                    Name = "api1"
                },
                new ApiScope
                {
                    Name = "api2"
                },
                new ApiScope
                {
                    Name = "api3-a"
                },
                new ApiScope
                {
                    Name = "api3-b"
                }
            };
        }
    }
}
