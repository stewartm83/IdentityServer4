// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;
using IdentityServer4.Services;
using IdentityServer4.Events;

namespace IdentityServer4.AspNetIdentity
{
    /// <summary>
    /// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class ResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
        where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<ResourceOwnerPasswordValidator<TUser>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOwnerPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        public ResourceOwnerPasswordValidator(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            ILogger<ResourceOwnerPasswordValidator<TUser>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
                if (result.Succeeded)
                {
                    var sub = await _userManager.GetUserIdAsync(user);

                    _logger.LogInformation("Credentials validated for username: {username}", context.UserName);

                    context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    return;
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: locked out", context.UserName);
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: not allowed", context.UserName);
                }
                else
                {
                    _logger.LogInformation("Authentication failed for username: {username}, reason: invalid credentials", context.UserName);
                }
            }
            else
            {
                _logger.LogInformation("No user found matching username: {username}", context.UserName);
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
}
