// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using IdentityServer4.ResponseHandling;

namespace IdentityServer4.Logging.Models
{
    internal class AuthorizeResponseLog
    {
        public string SubjectId { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

        public string Scope { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }


        public AuthorizeResponseLog(AuthorizeResponse response)
        {
            ClientId = response.Request?.Client?.ClientId;
            SubjectId = response.Request?.Subject?.GetSubjectId();
            RedirectUri = response.RedirectUri;
            State = response.State;
            Scope = response.Scope;
            Error = response.Error;
            ErrorDescription = response.ErrorDescription;
        }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}
