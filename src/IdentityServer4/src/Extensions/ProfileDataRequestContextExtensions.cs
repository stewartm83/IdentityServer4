// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Extensions for ProfileDataRequestContext
    /// </summary>
    public static class ProfileDataRequestContextExtensions
    {
        /// <summary>
        /// Filters the claims based on requested claim types.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public static List<Claim> FilterClaims(this ProfileDataRequestContext context, IEnumerable<Claim> claims)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            return claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
        }

        /// <summary>
        /// Filters the claims based on the requested claim types and then adds them to the IssuedClaims collection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="claims">The claims.</param>
        public static void AddRequestedClaims(this ProfileDataRequestContext context, IEnumerable<Claim> claims)
        {
            if (context.RequestedClaimTypes.Any())
            {
                context.IssuedClaims.AddRange(context.FilterClaims(claims));
            }
        }

        /// <summary>
        /// Logs the profile request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public static void LogProfileRequest(this ProfileDataRequestContext context, ILogger logger)
        {
            logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);
        }

        /// <summary>
        /// Logs the issued claims.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public static void LogIssuedClaims(this ProfileDataRequestContext context, ILogger logger)
        {
            logger.LogDebug("Issued claims: {claims}", context.IssuedClaims.Select(c => c.Type));
        }
    }
}
