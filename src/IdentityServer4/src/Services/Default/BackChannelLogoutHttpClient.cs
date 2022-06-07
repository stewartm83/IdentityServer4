// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Models making HTTP requests for back-channel logout notification.
    /// </summary>
    public class DefaultBackChannelLogoutHttpClient : IBackChannelLogoutHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<DefaultBackChannelLogoutHttpClient> _logger;

        /// <summary>
        /// Constructor for BackChannelLogoutHttpClient.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="loggerFactory"></param>
        public DefaultBackChannelLogoutHttpClient(HttpClient client, ILoggerFactory loggerFactory)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<DefaultBackChannelLogoutHttpClient>();
        }

        /// <summary>
        /// Posts the payload to the url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task PostAsync(string url, Dictionary<string, string> payload)
        {
            try
            {
                var response = await _client.PostAsync(url, new FormUrlEncodedContent(payload));
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Response from back-channel logout endpoint: {url} status code: {status}", url, (int)response.StatusCode);
                }
                else
                {
                    _logger.LogWarning("Response from back-channel logout endpoint: {url} status code: {status}", url, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception invoking back-channel logout for url: {url}", url);
            }
        }
    }
}
