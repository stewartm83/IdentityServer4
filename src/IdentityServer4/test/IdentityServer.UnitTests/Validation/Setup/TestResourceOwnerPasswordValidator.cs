// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class TestResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private string _erroDescription;
        private TokenRequestErrors _error;
        private readonly bool _sendError;

        public TestResourceOwnerPasswordValidator()
        { }

        public TestResourceOwnerPasswordValidator(TokenRequestErrors error, string errorDescription = null)
        {
            _sendError = true;
            _error = error;
            _erroDescription = errorDescription;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_sendError)
            {
                context.Result = new GrantValidationResult(_error, _erroDescription);
                return Task.CompletedTask;
            }

            if (context.UserName == context.Password)
            {
                context.Result = new GrantValidationResult(context.UserName, "password");
            }
            
            if (context.UserName == "bob_no_password" && context.Password == "")
            {
                context.Result = new GrantValidationResult(context.UserName, "password");
            }

            return Task.CompletedTask;
        }
    }
}
