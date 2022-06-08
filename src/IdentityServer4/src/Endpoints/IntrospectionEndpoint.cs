// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.ResponseHandling;
using Microsoft.Extensions.Logging;
using IdentityServer4.Hosting;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using System.Net;
using IdentityServer4.Services;
using IdentityServer4.Events;
using IdentityServer4.Extensions;

namespace IdentityServer4.Endpoints
{
    /// <summary>
    /// Introspection endpoint
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointHandler" />
    internal class IntrospectionEndpoint : IEndpointHandler
    {
        private readonly IIntrospectionResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger _logger;
        private readonly IIntrospectionRequestValidator _requestValidator;
        private readonly IApiSecretValidator _apiSecretValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionEndpoint" /> class.
        /// </summary>
        /// <param name="apiSecretValidator">The API secret validator.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <param name="responseGenerator">The generator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public IntrospectionEndpoint(
            IApiSecretValidator apiSecretValidator,
            IIntrospectionRequestValidator requestValidator,
            IIntrospectionResponseGenerator responseGenerator,
            IEventService events,
            ILogger<IntrospectionEndpoint> logger)
        {
            _apiSecretValidator = apiSecretValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogTrace("Processing introspection request.");

            // validate HTTP
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                _logger.LogWarning("Introspection endpoint only supports POST requests");
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            if (!context.Request.HasApplicationFormContentType())
            {
                _logger.LogWarning("Invalid media type for introspection endpoint");
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
            }

            return await ProcessIntrospectionRequestAsync(context);
        }

        private async Task<IEndpointResult> ProcessIntrospectionRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Starting introspection request.");

            // caller validation
            var apiResult = await _apiSecretValidator.ValidateAsync(context);
            if (apiResult.Resource == null)
            {
                _logger.LogError("API unauthorized to call introspection endpoint. aborting.");
                return new StatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var body = await context.Request.ReadFormAsync();
            if (body == null)
            {
                _logger.LogError("Malformed request body. aborting.");
                await _events.RaiseAsync(new TokenIntrospectionFailureEvent(apiResult.Resource.Name, "Malformed request body"));

                return new StatusCodeResult(HttpStatusCode.BadRequest);
            }

            // request validation
            _logger.LogTrace("Calling into introspection request validator: {type}", _requestValidator.GetType().FullName);
            var validationResult = await _requestValidator.ValidateAsync(body.AsNameValueCollection(), apiResult.Resource);
            if (validationResult.IsError)
            {
                LogFailure(validationResult.Error, apiResult.Resource.Name);
                await _events.RaiseAsync(new TokenIntrospectionFailureEvent(apiResult.Resource.Name, validationResult.Error));

                return new BadRequestResult(validationResult.Error);
            }

            // response generation
            _logger.LogTrace("Calling into introspection response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(validationResult);

            // render result
            LogSuccess(validationResult.IsActive, validationResult.Api.Name);
            return new IntrospectionResult(response);
        }

        private void LogSuccess(bool tokenActive, string apiName)
        {
            _logger.LogInformation("Success token introspection. Token active: {tokenActive}, for API name: {apiName}", tokenActive, apiName);
        }

        private void LogFailure(string error, string apiName)
        {
            _logger.LogError("Failed token introspection: {error}, for API name: {apiName}", error, apiName);
        }
    }
}
