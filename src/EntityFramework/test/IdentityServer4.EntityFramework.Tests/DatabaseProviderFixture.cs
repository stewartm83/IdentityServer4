// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.IntegrationTests
{
    /// <summary>
    /// xUnit ClassFixture for creating and deleting integration test databases.
    /// </summary>
    /// <typeparam name="T">DbContext of Type T</typeparam>
    public class DatabaseProviderFixture<T> : IDisposable where T : DbContext
    {
        public object StoreOptions;
        public List<DbContextOptions<T>> Options;

        public void Dispose()
        {
            if (Options != null) // null check since fixtures are created even when tests are skipped
            {
                foreach (var option in Options.ToList())
                {
                    using (var context = (T)Activator.CreateInstance(typeof(T), option, StoreOptions))
                    {
                        context.Database.EnsureDeleted();
                    }
                }
            }
        }
    }
}
