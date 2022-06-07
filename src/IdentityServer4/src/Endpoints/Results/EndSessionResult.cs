// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Extensions;
using System;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.Endpoints.Results
{
    /// <summary>
    /// Result for endsession
    /// </summary>
    /// <seealso cref="IdentityServer4.Hosting.IEndpointResult" />
    public class EndSessionResult : IEndpointResult
    {
        private readonly EndSessionValidationResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndSessionResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="System.ArgumentNullException">result</exception>
        public EndSessionResult(EndSessionValidationResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        internal EndSessionResult(
            EndSessionValidationResult result,
            IdentityServerOptions options,
            ISystemClock clock,
            IMessageStore<LogoutMessage> logoutMessageStore)
            : this(result)
        {
            _options = options;
            _clock = clock;
            _logoutMessageStore = logoutMessageStore;
        }

        private IdentityServerOptions _options;
        private ISystemClock _clock;
        private IMessageStore<LogoutMessage> _logoutMessageStore;

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _clock = _clock ?? context.RequestServices.GetRequiredService<ISystemClock>();
            _logoutMessageStore = _logoutMessageStore ?? context.RequestServices.GetRequiredService<IMessageStore<LogoutMessage>>();
        }

        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            var validatedRequest = _result.IsError ? null : _result.ValidatedRequest;

            string id = null;

            if (validatedRequest != null)
            {
                var logoutMessage = new LogoutMessage(validatedRequest);
                if (logoutMessage.ContainsPayload)
                {
                    var msg = new Message<LogoutMessage>(logoutMessage, _clock.UtcNow.UtcDateTime);
                    id = await _logoutMessageStore.WriteAsync(msg);
                }
            }

            var redirect = _options.UserInteraction.LogoutUrl;

            if (redirect.IsLocalUrl())
            {
                redirect = context.GetIdentityServerRelativeUrl(redirect);
            }

            if (id != null)
            {
                redirect = redirect.AddQueryString(_options.UserInteraction.LogoutIdParameter, id);
            }

            context.Response.Redirect(redirect);
        }
    }
}
