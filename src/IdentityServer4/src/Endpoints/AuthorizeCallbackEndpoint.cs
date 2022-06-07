// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Endpoints
{
    internal class AuthorizeCallbackEndpoint : AuthorizeEndpointBase
    {
        private readonly IConsentMessageStore _consentResponseStore;
        private readonly IAuthorizationParametersMessageStore _authorizationParametersMessageStore;

        public AuthorizeCallbackEndpoint(
            IEventService events,
            ILogger<AuthorizeCallbackEndpoint> logger,
            IdentityServerOptions options,
            IAuthorizeRequestValidator validator,
            IAuthorizeInteractionResponseGenerator interactionGenerator,
            IAuthorizeResponseGenerator authorizeResponseGenerator,
            IUserSession userSession,
            IConsentMessageStore consentResponseStore,
            IAuthorizationParametersMessageStore authorizationParametersMessageStore = null)
            : base(events, logger, options, validator, interactionGenerator, authorizeResponseGenerator, userSession)
        {
            _consentResponseStore = consentResponseStore;
            _authorizationParametersMessageStore = authorizationParametersMessageStore;
        }

        public override async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method))
            {
                Logger.LogWarning("Invalid HTTP method for authorize endpoint.");
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            Logger.LogDebug("Start authorize callback request");

            var parameters = context.Request.Query.AsNameValueCollection();
            if (_authorizationParametersMessageStore != null)
            {
                var messageStoreId = parameters[Constants.AuthorizationParamsStore.MessageStoreIdParameterName];
                var entry = await _authorizationParametersMessageStore.ReadAsync(messageStoreId);
                parameters = entry?.Data.FromFullDictionary() ?? new NameValueCollection();

                await _authorizationParametersMessageStore.DeleteAsync(messageStoreId);
            }

            var user = await UserSession.GetUserAsync();
            var consentRequest = new ConsentRequest(parameters, user?.GetSubjectId());
            var consent = await _consentResponseStore.ReadAsync(consentRequest.Id);

            if (consent != null && consent.Data == null)
            {
                return await CreateErrorResultAsync("consent message is missing data");
            }

            try
            {
                var result = await ProcessAuthorizeRequestAsync(parameters, user, consent?.Data);

                Logger.LogTrace("End Authorize Request. Result type: {0}", result?.GetType().ToString() ?? "-none-");

                return result;
            }
            finally
            {
                if (consent != null)
                {
                    await _consentResponseStore.DeleteAsync(consentRequest.Id);
                }
            }
        }
    }
}
