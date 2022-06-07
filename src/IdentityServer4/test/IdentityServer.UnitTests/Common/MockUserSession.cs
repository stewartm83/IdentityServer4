// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.UnitTests.Common
{
    public class MockUserSession : IUserSession
    {
        public List<string> Clients = new List<string>();

        public bool EnsureSessionIdCookieWasCalled { get; set; }
        public bool RemoveSessionIdCookieWasCalled { get; set; }
        public bool CreateSessionIdWasCalled { get; set; }

        public ClaimsPrincipal User { get; set; }
        public string SessionId { get; set; }
        public AuthenticationProperties Properties { get; set; }


        public Task<string> CreateSessionIdAsync(ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            CreateSessionIdWasCalled = true;
            User = principal;
            SessionId = Guid.NewGuid().ToString();
            return Task.FromResult(SessionId);
        }

        public Task<ClaimsPrincipal> GetUserAsync()
        {
            return Task.FromResult(User);
        }

        Task<string> IUserSession.GetSessionIdAsync()
        {
            return Task.FromResult(SessionId);
        }

        public Task EnsureSessionIdCookieAsync()
        {
            EnsureSessionIdCookieWasCalled = true;
            return Task.CompletedTask;
        }

        public Task RemoveSessionIdCookieAsync()
        {
            RemoveSessionIdCookieWasCalled = true;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetClientListAsync()
        {
            return Task.FromResult<IEnumerable<string>>(Clients);
        }

        public Task AddClientIdAsync(string clientId)
        {
            Clients.Add(clientId);
            return Task.CompletedTask;
        }
    }
}
