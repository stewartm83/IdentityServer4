// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Endpoints
{
    /// <summary>
    /// The device authorization endpoint
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointHandler" />
    internal class DeviceAuthorizationEndpoint : IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly IDeviceAuthorizationRequestValidator _requestValidator;
        private readonly IDeviceAuthorizationResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger<DeviceAuthorizationEndpoint> _logger;

        public DeviceAuthorizationEndpoint(
            IClientSecretValidator clientValidator,
            IDeviceAuthorizationRequestValidator requestValidator,
            IDeviceAuthorizationResponseGenerator responseGenerator,
            IEventService events,
            ILogger<DeviceAuthorizationEndpoint> logger)
        {
            _clientValidator = clientValidator;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogTrace("Processing device authorize request.");

            // validate HTTP
            if (!HttpMethods.IsPost(context.Request.Method) || !context.Request.HasApplicationFormContentType())
            {
                _logger.LogWarning("Invalid HTTP request for device authorize endpoint");
                return Error(OidcConstants.TokenErrors.InvalidRequest);
            }

            return await ProcessDeviceAuthorizationRequestAsync(context);
        }

        private async Task<IEndpointResult> ProcessDeviceAuthorizationRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Start device authorize request.");

            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null) return Error(OidcConstants.TokenErrors.InvalidClient);

            // validate request
            var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            var requestResult = await _requestValidator.ValidateAsync(form, clientResult);

            if (requestResult.IsError)
            {
                await _events.RaiseAsync(new DeviceAuthorizationFailureEvent(requestResult));
                return Error(requestResult.Error, requestResult.ErrorDescription);
            }

            var baseUrl = context.GetIdentityServerBaseUrl().EnsureTrailingSlash();

            // create response
            _logger.LogTrace("Calling into device authorize response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(requestResult, baseUrl);

            await _events.RaiseAsync(new DeviceAuthorizationSuccessEvent(response, requestResult));

            // return result
            _logger.LogDebug("Device authorize request success.");
            return new DeviceAuthorizationResult(response);
        }

        private TokenErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            _logger.LogError("Device authorization error: {error}:{errorDescriptions}", error, error ?? "-no message-");

            return new TokenErrorResult(response);
        }

        private void LogResponse(DeviceAuthorizationResponse response, DeviceAuthorizationRequestValidationResult requestResult)
        {
            var clientId = $"{requestResult.ValidatedRequest.Client.ClientId} ({requestResult.ValidatedRequest.Client?.ClientName ?? "no name set"})";

            if (response.DeviceCode != null)
            {
                _logger.LogTrace("Device code issued for {clientId}: {deviceCode}", clientId, response.DeviceCode);
            }
            if (response.UserCode != null)
            {
                _logger.LogTrace("User code issued for {clientId}: {userCode}", clientId, response.UserCode);
            }
            if (response.VerificationUri != null)
            {
                _logger.LogTrace("Verification URI issued for {clientId}: {verificationUri}", clientId, response.VerificationUri);
            }
            if (response.VerificationUriComplete != null)
            {
                _logger.LogTrace("Verification URI (Complete) issued for {clientId}: {verificationUriComplete}", clientId, response.VerificationUriComplete);
            }
        }
    }
}
