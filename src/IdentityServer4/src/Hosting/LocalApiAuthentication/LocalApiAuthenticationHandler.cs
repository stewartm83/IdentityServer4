// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IdentityServer4.Hosting.LocalApiAuthentication
{
    /// <summary>
    /// Authentication handler for validating access token from the local IdentityServer
    /// </summary>
    public class LocalApiAuthenticationHandler : AuthenticationHandler<LocalApiAuthenticationOptions>
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly ILogger _logger;

        /// <inheritdoc />
        public LocalApiAuthenticationHandler(IOptionsMonitor<LocalApiAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenValidator tokenValidator)
            : base(options, logger, encoder, clock)
        {
            _tokenValidator = tokenValidator;
            _logger = logger.CreateLogger<LocalApiAuthenticationHandler>();
        }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new LocalApiAuthenticationEvents Events
        {
            get => (LocalApiAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        /// <inheritdoc/>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new LocalApiAuthenticationEvents());

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogTrace("HandleAuthenticateAsync called");

            string token = null;

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.NoResult();
            }

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("No Access Token is sent.");
            }

            _logger.LogTrace("Token found: {token}", token);

            TokenValidationResult result = await _tokenValidator.ValidateAccessTokenAsync(token, Options.ExpectedScope);

            if (result.IsError)
            {
                _logger.LogTrace("Failed to validate the token");

                return AuthenticateResult.Fail(result.Error);
            }

            _logger.LogTrace("Successfully validated the token.");

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(result.Claims, Scheme.Name, JwtClaimTypes.Name, JwtClaimTypes.Role);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationProperties authenticationProperties = new AuthenticationProperties();

            if (Options.SaveToken)
            {
                authenticationProperties.StoreTokens(new[]
                {
                    new AuthenticationToken { Name = "access_token", Value = token }
                });
            }

            var claimsTransformationContext = new ClaimsTransformationContext
            {
                Principal = claimsPrincipal,
                HttpContext = Context
            };

            await Events.ClaimsTransformation(claimsTransformationContext);

            AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsTransformationContext.Principal, authenticationProperties, Scheme.Name);
            return AuthenticateResult.Success(authenticationTicket);
        }
    }
}
