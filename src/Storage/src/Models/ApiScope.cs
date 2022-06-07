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
    /// Models access to an API scope
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ApiScope : Resource
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(ApiScope)}}}";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScope"/> class.
        /// </summary>
        public ApiScope()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScope"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ApiScope(string name)
            : this(name, name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScope"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        public ApiScope(string name, string displayName)
            : this(name, displayName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScope"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="userClaims">List of associated user claims that should be included when this resource is requested.</param>
        public ApiScope(string name, IEnumerable<string> userClaims)
            : this(name, name, userClaims)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScope"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="userClaims">List of associated user claims that should be included when this resource is requested.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public ApiScope(string name, string displayName, IEnumerable<string> userClaims)
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
        /// Specifies whether the user can de-select the scope on the consent screen. Defaults to false.
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Specifies whether the consent screen will emphasize this scope. Use this setting for sensitive or important scopes. Defaults to false.
        /// </summary>
        public bool Emphasize { get; set; } = false;
    }
}
