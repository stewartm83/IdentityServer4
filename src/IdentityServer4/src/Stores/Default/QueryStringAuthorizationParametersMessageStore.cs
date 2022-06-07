// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using IdentityServer4.Models;
using System.Threading.Tasks;
using IdentityServer4.Extensions;

namespace IdentityServer4.Stores
{
    // internal just for testing
    internal class QueryStringAuthorizationParametersMessageStore : IAuthorizationParametersMessageStore
    {
        public Task<string> WriteAsync(Message<IDictionary<string, string[]>> message)
        {
            var queryString = message.Data.FromFullDictionary().ToQueryString();
            return Task.FromResult(queryString);
        }

        public Task<Message<IDictionary<string, string[]>>> ReadAsync(string id)
        {
            var values = id.ReadQueryStringAsNameValueCollection();
            var msg = new Message<IDictionary<string, string[]>>(values.ToFullDictionary());
            return Task.FromResult(msg);
        }

        public Task DeleteAsync(string id)
        {
            return Task.CompletedTask;
        }
    }
}
