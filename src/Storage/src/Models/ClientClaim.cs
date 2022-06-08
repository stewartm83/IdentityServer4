// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Security.Claims;

namespace IdentityServer4.Models
{
    /// <summary>
    /// A client claim
    /// </summary>
    public class ClientClaim
    {
        /// <summary>
        /// The claim type
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// The claim value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The claim value type
        /// </summary>
        public string ValueType { get; set; } = ClaimValueTypes.String;

        /// <summary>
        /// ctor
        /// </summary>
        public ClientClaim()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public ClientClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        public ClientClaim(string type, string value, string valueType)
        {
            Type = type;
            Value = value;
            ValueType = valueType;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;

                hash = hash * 23 + Value.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + ValueType.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is ClientClaim c)
            {
                return (string.Equals(Type, c.Type, StringComparison.Ordinal) &&
                        string.Equals(Value, c.Value, StringComparison.Ordinal) &&
                        string.Equals(ValueType, c.ValueType, StringComparison.Ordinal));
            }

            return false;
        }
    }
}
