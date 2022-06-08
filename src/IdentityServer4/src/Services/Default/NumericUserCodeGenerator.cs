// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    /// <summary>
    /// User code generator using 9 digit number
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IUserCodeGenerator" />
    public class NumericUserCodeGenerator : IUserCodeGenerator
    {
        /// <summary>
        /// Gets the type of the user code.
        /// </summary>
        /// <value>
        /// The type of the user code.
        /// </value>
        public string UserCodeType => IdentityServerConstants.UserCodeTypes.Numeric;

        /// <summary>
        /// Gets the retry limit.
        /// </summary>
        /// <value>
        /// The retry limit for getting a unique value.
        /// </value>
        public int RetryLimit => 5;

        /// <summary>
        /// Generates the user code.
        /// </summary>
        /// <returns></returns>
        public Task<string> GenerateAsync()
        {
            var next = Next(100000000, 999999999);
            return Task.FromResult(next.ToString());
        }

        private int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue) return minValue;
            long diff = maxValue - minValue;

            var uint32Buffer = new byte[8];

            using (var rng = new RNGCryptoServiceProvider())
            {
                while (true)
                {
                    rng.GetBytes(uint32Buffer);
                    var rand = BitConverter.ToUInt32(uint32Buffer, 0);

                    const long max = 1 + (long)uint.MaxValue;
                    var remainder = max % diff;
                    if (rand < max - remainder)
                    {
                        return (int)(minValue + rand % diff);
                    }
                }
            }
        }
    }
}
