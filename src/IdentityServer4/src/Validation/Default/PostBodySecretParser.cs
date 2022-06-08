// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Parses a POST body for secrets
    /// </summary>
    public class PostBodySecretParser : ISecretParser
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;

        /// <summary>
        /// Creates the parser with options
        /// </summary>
        /// <param name="options">IdentityServer options</param>
        /// <param name="logger">Logger</param>
        public PostBodySecretParser(IdentityServerOptions options, ILogger<PostBodySecretParser> logger)
        {
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Returns the authentication method name that this parser implements
        /// </summary>
        /// <value>
        /// The authentication method.
        /// </value>
        public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.PostBody;

        /// <summary>
        /// Tries to find a secret on the context that can be used for authentication
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>
        /// A parsed secret
        /// </returns>
        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for secret in post body");

            if (!context.Request.HasApplicationFormContentType())
            {
                _logger.LogDebug("Content type is not a form");
                return null;
            }

            var body = await context.Request.ReadFormAsync();

            if (body != null)
            {
                var id = body["client_id"].FirstOrDefault();
                var secret = body["client_secret"].FirstOrDefault();

                // client id must be present
                if (id.IsPresent())
                {
                    if (id.Length > _options.InputLengthRestrictions.ClientId)
                    {
                        _logger.LogError("Client ID exceeds maximum length.");
                        return null;
                    }

                    if (secret.IsPresent())
                    {
                        if (secret.Length > _options.InputLengthRestrictions.ClientSecret)
                        {
                            _logger.LogError("Client secret exceeds maximum length.");
                            return null;
                        }

                        return new ParsedSecret
                        {
                            Id = id,
                            Credential = secret,
                            Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
                        };
                    }
                    else
                    {
                        // client secret is optional
                        _logger.LogDebug("client id without secret found");

                        return new ParsedSecret
                        {
                            Id = id,
                            Type = IdentityServerConstants.ParsedSecretTypes.NoSecret
                        };
                    }
                }
            }

            _logger.LogDebug("No secret in post body found");
            return null;
        }
    }
}
