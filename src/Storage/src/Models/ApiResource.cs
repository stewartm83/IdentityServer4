// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models a web API resource.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ApiResource : Resource
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(ApiResource)}}}";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        public ApiResource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ApiResource(string name)
            : this(name, name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        public ApiResource(string name, string displayName)
            : this(name, displayName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="userClaims">List of associated user claims that should be included when this resource is requested.</param>
        public ApiResource(string name, IEnumerable<string> userClaims)
            : this(name, name, userClaims)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="userClaims">List of associated user claims that should be included when this resource is requested.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public ApiResource(string name, string displayName, IEnumerable<string> userClaims)
        {
            if (name.IsMissing()) throw new ArgumentNullException(nameof(name));

            Name = name;
            DisplayName = displayName;

            if (!userClaims.IsNullOrEmpty())
            {
                foreach (var type in userClaims)
                {
                    UserClaims.Add(type);
                }
            }
        }

        /// <summary>
        /// The API secret is used for the introspection endpoint. The API can authenticate with introspection using the API name and secret.
        /// </summary>
        public ICollection<Secret> ApiSecrets { get; set; } = new HashSet<Secret>();

        /// <summary>
        /// Models the scopes this API resource allows.
        /// </summary>
        public ICollection<string> Scopes { get; set; } = new HashSet<string>();

        /// <summary>
        /// Signing algorithm for access token. If empty, will use the server default signing algorithm.
        /// </summary>
        public ICollection<string> AllowedAccessTokenSigningAlgorithms { get; set; } = new HashSet<string>();
    }
}
