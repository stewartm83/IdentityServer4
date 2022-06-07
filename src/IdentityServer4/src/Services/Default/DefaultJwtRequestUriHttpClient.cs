// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Default JwtRequest client
    /// </summary>
    public class DefaultJwtRequestUriHttpClient : IJwtRequestUriHttpClient
    {
        private readonly HttpClient _client;
        private readonly IdentityServerOptions _options;
        private readonly ILogger<DefaultJwtRequestUriHttpClient> _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="client">An HTTP client</param>
        /// <param name="options">The options.</param>
        /// <param name="loggerFactory">The logger factory</param>
        public DefaultJwtRequestUriHttpClient(HttpClient client, IdentityServerOptions options, ILoggerFactory loggerFactory)
        {
            _client = client;
            _options = options;
            _logger = loggerFactory.CreateLogger<DefaultJwtRequestUriHttpClient>();
        }


        /// <inheritdoc />
        public async Task<string> GetJwtAsync(string url, Client client)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Properties.Add(IdentityServerConstants.JwtRequestClientKey, client);

            var response = await _client.SendAsync(req);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (_options.StrictJarValidation)
                {
                    if (!string.Equals(response.Content.Headers.ContentType.MediaType,
                        $"application/{JwtClaimTypes.JwtTypes.AuthorizationRequest}", StringComparison.Ordinal))
                    {
                        _logger.LogError("Invalid content type {type} from jwt url {url}", response.Content.Headers.ContentType.MediaType, url);
                        return null;
                    }
                }

                _logger.LogDebug("Success http response from jwt url {url}", url);
                
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
                
            _logger.LogError("Invalid http status code {status} from jwt url {url}", response.StatusCode, url);
            return null;
        }
    }
}
