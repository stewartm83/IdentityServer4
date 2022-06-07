// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validates an incoming token request using the device flow
    /// </summary>
    internal class DeviceCodeValidator : IDeviceCodeValidator
    {
        private readonly IDeviceFlowCodeService _devices;
        private readonly IProfileService _profile;
        private readonly IDeviceFlowThrottlingService _throttlingService;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<DeviceCodeValidator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCodeValidator"/> class.
        /// </summary>
        /// <param name="devices">The devices.</param>
        /// <param name="profile">The profile.</param>
        /// <param name="throttlingService">The throttling service.</param>
        /// <param name="systemClock">The system clock.</param>
        /// <param name="logger">The logger.</param>
        public DeviceCodeValidator(
            IDeviceFlowCodeService devices,
            IProfileService profile,
            IDeviceFlowThrottlingService throttlingService,
            ISystemClock systemClock,
            ILogger<DeviceCodeValidator> logger)
        {
            _devices = devices;
            _profile = profile;
            _throttlingService = throttlingService;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <summary>
        /// Validates the device code.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(DeviceCodeValidationContext context)
        {
            var deviceCode = await _devices.FindByDeviceCodeAsync(context.DeviceCode);

            if (deviceCode == null)
            {
                _logger.LogError("Invalid device code");
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.InvalidGrant);
                return;
            }
            
            // validate client binding
            if (deviceCode.ClientId != context.Request.Client.ClientId)
            {
                _logger.LogError("Client {0} is trying to use a device code from client {1}", context.Request.Client.ClientId, deviceCode.ClientId);
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.InvalidGrant);
                return;
            }

            if (await _throttlingService.ShouldSlowDown(context.DeviceCode, deviceCode))
            {
                _logger.LogError("Client {0} is polling too fast", deviceCode.ClientId);
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.SlowDown);
                return;
            }

            // validate lifetime
            if (deviceCode.CreationTime.AddSeconds(deviceCode.Lifetime) < _systemClock.UtcNow)
            {
                _logger.LogError("Expired device code");
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.ExpiredToken);
                return;
            }

            // denied
            if (deviceCode.IsAuthorized
                && (deviceCode.AuthorizedScopes == null || deviceCode.AuthorizedScopes.Any() == false))
            {
                _logger.LogError("No scopes authorized for device authorization. Access denied");
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.AccessDenied);
                return;
            }

            // make sure code is authorized
            if (!deviceCode.IsAuthorized || deviceCode.Subject == null)
            {
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.AuthorizationPending);
                return;
            }

            // make sure user is enabled
            var isActiveCtx = new IsActiveContext(deviceCode.Subject, context.Request.Client, IdentityServerConstants.ProfileIsActiveCallers.DeviceCodeValidation);
            await _profile.IsActiveAsync(isActiveCtx);

            if (isActiveCtx.IsActive == false)
            {
                _logger.LogError("User has been disabled: {subjectId}", deviceCode.Subject.GetSubjectId());
                context.Result = new TokenRequestValidationResult(context.Request, OidcConstants.TokenErrors.InvalidGrant);
                return;
            }

            context.Request.DeviceCode = deviceCode;
            context.Request.SessionId = deviceCode.SessionId;

            context.Result = new TokenRequestValidationResult(context.Request);
            await _devices.RemoveByDeviceCodeAsync(context.DeviceCode);
        }
    }
}
