// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Uses the registered secret parsers to parse a secret on the current request
    /// </summary>
    public class SecretParser : ISecretsListParser
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<ISecretParser> _parsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretParser"/> class.
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        /// <param name="logger">The logger.</param>
        public SecretParser(IEnumerable<ISecretParser> parsers, ILogger<ISecretsListParser> logger)
        {
            _parsers = parsers;
            _logger = logger;
        }

        /// <summary>
        /// Checks the context to find a secret.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            // see if a registered parser finds a secret on the request
            ParsedSecret bestSecret = null;
            foreach (var parser in _parsers)
            {
                var parsedSecret = await parser.ParseAsync(context);
                if (parsedSecret != null)
                {
                    _logger.LogDebug("Parser found secret: {type}", parser.GetType().Name);

                    bestSecret = parsedSecret;

                    if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.NoSecret)
                    {
                        break;
                    }
                }
            }

            if (bestSecret != null)
            {
                _logger.LogDebug("Secret id found: {id}", bestSecret.Id);
                return bestSecret;
            }

            _logger.LogDebug("Parser found no secret");
            return null;
        }

        /// <summary>
        /// Gets all available authentication methods.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAvailableAuthenticationMethods()
        {
            return _parsers.Select(p => p.AuthenticationMethod).Where(p => !String.IsNullOrWhiteSpace(p));
        }
    }
}
