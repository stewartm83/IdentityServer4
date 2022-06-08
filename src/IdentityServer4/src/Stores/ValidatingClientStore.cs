// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// Client store decorator for running runtime configuration validation checks
    /// </summary>
    public class ValidatingClientStore<T> : IClientStore
        where T : IClientStore
    {
        private readonly IClientStore _inner;
        private readonly IClientConfigurationValidator _validator;
        private readonly IEventService _events;
        private readonly ILogger<ValidatingClientStore<T>> _logger;
        private readonly string _validatorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingClientStore{T}" /> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public ValidatingClientStore(T inner, IClientConfigurationValidator validator, IEventService events, ILogger<ValidatingClientStore<T>> logger)
        {
            _inner = inner;
            _validator = validator;
            _events = events;
            _logger = logger;

            _validatorType = validator.GetType().FullName;
        }

        /// <summary>
        /// Finds a client by id (and runs the validation logic)
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client or an InvalidOperationException
        /// </returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _inner.FindClientByIdAsync(clientId);

            if (client != null)
            {
                _logger.LogTrace("Calling into client configuration validator: {validatorType}", _validatorType);

                var context = new ClientConfigurationValidationContext(client);
                await _validator.ValidateAsync(context);

                if (context.IsValid)
                {
                    _logger.LogDebug("client configuration validation for client {clientId} succeeded.", client.ClientId);
                    return client;
                }

                _logger.LogError("Invalid client configuration for client {clientId}: {errorMessage}", client.ClientId, context.ErrorMessage);
                await _events.RaiseAsync(new InvalidClientConfigurationEvent(client, context.ErrorMessage));
                    
                return null;
            }

            return null;
        }
    }
}
