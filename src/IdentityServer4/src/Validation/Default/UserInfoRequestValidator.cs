// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Default userinfo request validator
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IUserInfoRequestValidator" />
    internal class UserInfoRequestValidator : IUserInfoRequestValidator
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IProfileService _profile;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoRequestValidator" /> class.
        /// </summary>
        /// <param name="tokenValidator">The token validator.</param>
        /// <param name="profile">The profile service</param>
        /// <param name="logger">The logger.</param>
        public UserInfoRequestValidator(
            ITokenValidator tokenValidator, 
            IProfileService profile,
            ILogger<UserInfoRequestValidator> logger)
        {
            _tokenValidator = tokenValidator;
            _profile = profile;
            _logger = logger;
        }

        /// <summary>
        /// Validates a userinfo request.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<UserInfoRequestValidationResult> ValidateRequestAsync(string accessToken)
        {
            // the access token needs to be valid and have at least the openid scope
            var tokenResult = await _tokenValidator.ValidateAccessTokenAsync(
                accessToken,
                IdentityServerConstants.StandardScopes.OpenId);

            if (tokenResult.IsError)
            {
                return new UserInfoRequestValidationResult
                {
                    IsError = true,
                    Error = tokenResult.Error
                };
            }

            // the token must have a one sub claim
            var subClaim = tokenResult.Claims.SingleOrDefault(c => c.Type == JwtClaimTypes.Subject);
            if (subClaim == null)
            {
                _logger.LogError("Token contains no sub claim");

                return new UserInfoRequestValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.ProtectedResourceErrors.InvalidToken
                };
            }

            // create subject from incoming access token
            var claims = tokenResult.Claims.Where(x => !Constants.Filters.ProtocolClaimsFilter.Contains(x.Type));
            var subject = Principal.Create("UserInfo", claims.ToArray());

            // make sure user is still active
            var isActiveContext = new IsActiveContext(subject, tokenResult.Client, IdentityServerConstants.ProfileIsActiveCallers.UserInfoRequestValidation);
            await _profile.IsActiveAsync(isActiveContext);

            if (isActiveContext.IsActive == false)
            {
                _logger.LogError("User is not active: {sub}", subject.GetSubjectId());

                return new UserInfoRequestValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.ProtectedResourceErrors.InvalidToken
                };
            }

            return new UserInfoRequestValidationResult
            {
                IsError = false,
                TokenValidationResult = tokenResult,
                Subject = subject
            };
        }
    }
}
