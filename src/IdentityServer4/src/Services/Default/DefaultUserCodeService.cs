// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Default user code service implementation.
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IUserCodeService" />
    public class DefaultUserCodeService : IUserCodeService
    {
        private readonly IEnumerable<IUserCodeGenerator> _generators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUserCodeService"/> class.
        /// </summary>
        /// <param name="generators">The generators.</param>
        /// <exception cref="ArgumentNullException">generators</exception>
        public DefaultUserCodeService(IEnumerable<IUserCodeGenerator> generators)
        {
            _generators = generators ?? throw new ArgumentNullException(nameof(generators));
        }

        /// <summary>
        /// Gets the user code generator.
        /// </summary>
        /// <param name="userCodeType">Type of user code.</param>
        /// <returns></returns>
        public Task<IUserCodeGenerator> GetGenerator(string userCodeType) =>
            Task.FromResult(_generators.FirstOrDefault(x => x.UserCodeType == userCodeType));
    }
}
