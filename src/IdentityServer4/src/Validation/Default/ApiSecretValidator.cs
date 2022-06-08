// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates API secrets using the registered secret validators and parsers
    /// </summary>
    public class ApiSecretValidator : IApiSecretValidator
    {
        private readonly ILogger _logger;
        private readonly IResourceStore _resources;
        private readonly IEventService _events;
        private readonly ISecretsListParser _parser;
        private readonly ISecretsListValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSecretValidator"/> class.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="parsers">The parsers.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public ApiSecretValidator(IResourceStore resources, ISecretsListParser parsers, ISecretsListValidator validator, IEventService events, ILogger<ApiSecretValidator> logger)
        {
            _resources = resources;
            _parser = parsers;
            _validator = validator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Validates the secret on the current request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<ApiSecretValidationResult> ValidateAsync(HttpContext context)
        {
            _logger.LogTrace("Start API validation");

            var fail = new ApiSecretValidationResult
            {
                IsError = true
            };

            var parsedSecret = await _parser.ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No API id or secret found");

                _logger.LogError("No API secret found");
                return fail;
            }

            // load API resource
            var apis = await _resources.FindApiResourcesByNameAsync(new[] { parsedSecret.Id });
            if (apis == null || !apis.Any())
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown API resource");

                _logger.LogError("No API resource with that name found. aborting");
                return fail;
            }

            if (apis.Count() > 1)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Invalid API resource");

                _logger.LogError("More than one API resource with that name found. aborting");
                return fail;
            }

            var api = apis.Single();

            if (api.Enabled == false)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "API resource not enabled");

                _logger.LogError("API resource not enabled. aborting.");
                return fail;
            }

            var result = await _validator.ValidateAsync(api.ApiSecrets, parsedSecret);
            if (result.Success)
            {
                _logger.LogDebug("API resource validation success");

                var success = new ApiSecretValidationResult
                {
                    IsError = false,
                    Resource = api
                };

                await RaiseSuccessEventAsync(api.Name, parsedSecret.Type);
                return success;
            }

            await RaiseFailureEventAsync(api.Name, "Invalid API secret");
            _logger.LogError("API validation failed.");

            return fail;
        }

        private Task RaiseSuccessEventAsync(string clientId, string authMethod)
        {
            return _events.RaiseAsync(new ApiAuthenticationSuccessEvent(clientId, authMethod));
        }

        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ApiAuthenticationFailureEvent(clientId, message));
        }
    }
}
