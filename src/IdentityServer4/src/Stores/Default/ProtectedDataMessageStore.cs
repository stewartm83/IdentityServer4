// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using IdentityModel;
using System.Text;
using System;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// IMessageStore implementation that uses data protection to protect message.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ProtectedDataMessageStore<TModel> : IMessageStore<TModel>
    {
        private const string Purpose = "IdentityServer4.Stores.ProtectedDataMessageStore";

        /// <summary>
        /// The data protector.
        /// </summary>
        protected readonly IDataProtector Protector;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="logger"></param>
        public ProtectedDataMessageStore(IDataProtectionProvider provider, ILogger<ProtectedDataMessageStore<TModel>> logger)
        {
            Protector = provider.CreateProtector(Purpose);
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual Task<Message<TModel>> ReadAsync(string value)
        {
            Message<TModel> result = null;

            if (!String.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var bytes = Base64Url.Decode(value);
                    bytes = Protector.Unprotect(bytes);
                    var json = Encoding.UTF8.GetString(bytes);
                    result = ObjectSerializer.FromString<Message<TModel>>(json);
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, "Exception reading protected message");
                }
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public virtual Task<string> WriteAsync(Message<TModel> message)
        {
            string value = null;

            try
            {
                var json = ObjectSerializer.ToString(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                bytes = Protector.Protect(bytes);
                value = Base64Url.Encode(bytes);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Exception writing protected message");
            }

            return Task.FromResult(value);
        }
    }
}
