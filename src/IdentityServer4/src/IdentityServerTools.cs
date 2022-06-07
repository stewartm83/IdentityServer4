// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System.Security.Claims;
using IdentityServer4.Services;
using IdentityModel;
using System;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4
{
    /// <summary>
    /// Class for useful helpers for interacting with IdentityServer
    /// </summary>
    public class IdentityServerTools
    {
        internal readonly IHttpContextAccessor ContextAccessor;
        private readonly ITokenCreationService _tokenCreation;
        private readonly ISystemClock _clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerTools" /> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="tokenCreation">The token creation service.</param>
        /// <param name="clock">The clock.</param>
        public IdentityServerTools(IHttpContextAccessor contextAccessor, ITokenCreationService tokenCreation, ISystemClock clock)
        {
            ContextAccessor = contextAccessor;
            _tokenCreation = tokenCreation;
            _clock = clock;
        }

        /// <summary>
        /// Issues a JWT.
        /// </summary>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">claims</exception>
        public virtual async Task<string> IssueJwtAsync(int lifetime, IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var issuer = ContextAccessor.HttpContext.GetIdentityServerIssuerUri();

            var token = new Token
            {
                CreationTime = _clock.UtcNow.UtcDateTime,
                Issuer = issuer,
                Lifetime = lifetime,

                Claims = new HashSet<Claim>(claims, new ClaimComparer())
            };

            return await _tokenCreation.CreateTokenAsync(token);
        }

        /// <summary>
        /// Issues a JWT.
        /// </summary>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">claims</exception>
        public virtual async Task<string> IssueJwtAsync(int lifetime, string issuer, IEnumerable<Claim> claims)
        {
            if (String.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var token = new Token
            {
                CreationTime = _clock.UtcNow.UtcDateTime,
                Issuer = issuer,
                Lifetime = lifetime,

                Claims = new HashSet<Claim>(claims, new ClaimComparer())
            };

            return await _tokenCreation.CreateTokenAsync(token);
        }
    }
}
