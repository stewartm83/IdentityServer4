// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using System.Linq;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Convenience class that defines standard identity resources.
    /// </summary>
    public static class IdentityResources
    {
        /// <summary>
        /// Models the standard openid scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class OpenId : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OpenId"/> class.
            /// </summary>
            public OpenId()
            {
                Name = IdentityServerConstants.StandardScopes.OpenId;
                DisplayName = "Your user identifier";
                Required = true;
                UserClaims.Add(JwtClaimTypes.Subject);
            }
        }

        /// <summary>
        /// Models the standard profile scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class Profile : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Profile"/> class.
            /// </summary>
            public Profile()
            {
                Name = IdentityServerConstants.StandardScopes.Profile;
                DisplayName = "User profile";
                Description = "Your user profile information (first name, last name, etc.)";
                Emphasize = true;
                UserClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Profile].ToList();
            }
        }

        /// <summary>
        /// Models the standard email scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class Email : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Email"/> class.
            /// </summary>
            public Email()
            {
                Name = IdentityServerConstants.StandardScopes.Email;
                DisplayName = "Your email address";
                Emphasize = true;
                UserClaims = (Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Email].ToList());
            }
        }

        /// <summary>
        /// Models the standard phone scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class Phone : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Phone"/> class.
            /// </summary>
            public Phone()
            {
                Name = IdentityServerConstants.StandardScopes.Phone;
                DisplayName = "Your phone number";
                Emphasize = true;
                UserClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Phone].ToList();
            }
        }

        /// <summary>
        /// Models the standard address scope
        /// </summary>
        /// <seealso cref="IdentityServer4.Models.IdentityResource" />
        public class Address : IdentityResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Address"/> class.
            /// </summary>
            public Address()
            {
                Name = IdentityServerConstants.StandardScopes.Address;
                DisplayName = "Your postal address";
                Emphasize = true;
                UserClaims = Constants.ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Address].ToList();
            }
        }
    }
}
