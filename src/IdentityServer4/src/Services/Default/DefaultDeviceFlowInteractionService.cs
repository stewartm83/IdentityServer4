// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Services
{
    internal class DefaultDeviceFlowInteractionService : IDeviceFlowInteractionService
    {
        private readonly IClientStore _clients;
        private readonly IUserSession _session;
        private readonly IDeviceFlowCodeService _devices;
        private readonly IResourceStore _resourceStore;
        private readonly IScopeParser _scopeParser;
        private readonly ILogger<DefaultDeviceFlowInteractionService> _logger;

        public DefaultDeviceFlowInteractionService(
            IClientStore clients,
            IUserSession session,
            IDeviceFlowCodeService devices,
            IResourceStore resourceStore,
            IScopeParser scopeParser,
            ILogger<DefaultDeviceFlowInteractionService> logger)
        {
            _clients = clients;
            _session = session;
            _devices = devices;
            _resourceStore = resourceStore;
            _scopeParser = scopeParser;
            _logger = logger;
        }

        public async Task<DeviceFlowAuthorizationRequest> GetAuthorizationContextAsync(string userCode)
        {
            var deviceAuth = await _devices.FindByUserCodeAsync(userCode);
            if (deviceAuth == null) return null;

            var client = await _clients.FindClientByIdAsync(deviceAuth.ClientId);
            if (client == null) return null;

            var parsedScopesResult = _scopeParser.ParseScopeValues(deviceAuth.RequestedScopes);
            var validatedResources = await _resourceStore.CreateResourceValidationResult(parsedScopesResult);

            return new DeviceFlowAuthorizationRequest
            {
                Client = client,
                ValidatedResources = validatedResources
            };
        }

        public async Task<DeviceFlowInteractionResult> HandleRequestAsync(string userCode, ConsentResponse consent)
        {
            if (userCode == null) throw new ArgumentNullException(nameof(userCode));
            if (consent == null) throw new ArgumentNullException(nameof(consent));
            
            var deviceAuth = await _devices.FindByUserCodeAsync(userCode);
            if (deviceAuth == null) return LogAndReturnError("Invalid user code", "Device authorization failure - user code is invalid");

            var client = await _clients.FindClientByIdAsync(deviceAuth.ClientId);
            if (client == null) return LogAndReturnError("Invalid client", "Device authorization failure - requesting client is invalid");

            var subject = await _session.GetUserAsync();
            if (subject == null) return LogAndReturnError("No user present in device flow request", "Device authorization failure - no user found");
            
            var sid = await _session.GetSessionIdAsync();

            deviceAuth.IsAuthorized = true;
            deviceAuth.Subject = subject;
            deviceAuth.SessionId = sid;
            deviceAuth.Description = consent.Description;
            deviceAuth.AuthorizedScopes = consent.ScopesValuesConsented;

            // TODO: Device Flow - Record consent template
            if (consent.RememberConsent)
            {
                //var consentRequest = new ConsentRequest(request, subject);
                //await _consentMessageStore.WriteAsync(consentRequest.Id, new Message<ConsentResponse>(consent, _clock.UtcNow.UtcDateTime));
            }

            await _devices.UpdateByUserCodeAsync(userCode, deviceAuth);

            return new DeviceFlowInteractionResult();
        }

        private DeviceFlowInteractionResult LogAndReturnError(string error, string errorDescription = null)
        {
            _logger.LogError(errorDescription);
            return DeviceFlowInteractionResult.Failure(error);
        }
    }
}
