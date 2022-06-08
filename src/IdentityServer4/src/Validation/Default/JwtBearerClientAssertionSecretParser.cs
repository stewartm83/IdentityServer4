// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Threading.Tasks;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Parses a POST body for a JWT bearer client assertion
    /// </summary>
    public class JwtBearerClientAssertionSecretParser : ISecretParser
    {
        private readonly IdentityServerOptions _options;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerClientAssertionSecretParser"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public JwtBearerClientAssertionSecretParser(IdentityServerOptions options, ILogger<JwtBearerClientAssertionSecretParser> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// Returns the authentication method name that this parser implements
        /// </summary>
        /// <value>
        /// The authentication method.
        /// </value>
        public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.PrivateKeyJwt;

        /// <summary>
        /// Tries to find a JWT client assertion token in the request body that can be used for authentication
        /// Used for "private_key_jwt" client authentication method as defined in http://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>
        /// A parsed secret
        /// </returns>
        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for JWT client assertion in post body");

            if (!context.Request.HasApplicationFormContentType())
            {
                _logger.LogDebug("Content type is not a form");
                return null;
            }

            var body = await context.Request.ReadFormAsync();

            if (body != null)
            {
                var clientAssertionType = body[OidcConstants.TokenRequest.ClientAssertionType].FirstOrDefault();
                var clientAssertion = body[OidcConstants.TokenRequest.ClientAssertion].FirstOrDefault();

                if (clientAssertion.IsPresent()
                    && clientAssertionType == OidcConstants.ClientAssertionTypes.JwtBearer)
                {
                    if (clientAssertion.Length > _options.InputLengthRestrictions.Jwt)
                    {
                        _logger.LogError("Client assertion token exceeds maximum length.");
                        return null;
                    }

                    var clientId = GetClientIdFromToken(clientAssertion);
                    if (!clientId.IsPresent())
                    {
                        return null;
                    }

                    if (clientId.Length > _options.InputLengthRestrictions.ClientId)
                    {
                        _logger.LogError("Client ID exceeds maximum length.");
                        return null;
                    }

                    var parsedSecret = new ParsedSecret
                    {
                        Id = clientId,
                        Credential = clientAssertion,
                        Type = IdentityServerConstants.ParsedSecretTypes.JwtBearer
                    };

                    return parsedSecret;
                }
            }

            _logger.LogDebug("No JWT client assertion found in post body");
            return null;
        }

        private string GetClientIdFromToken(string token)
        {
            try
            {
                var jwt = new JwtSecurityToken(token);
                return jwt.Subject;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Could not parse client assertion", e);
                return null;
            }
        }
    }
}
