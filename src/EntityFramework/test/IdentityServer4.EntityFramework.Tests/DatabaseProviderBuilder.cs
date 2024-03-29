﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.IntegrationTests
{
    /// <summary>
    /// Helper methods to initialize DbContextOptions for the specified database provider and context.
    /// </summary>
    public class DatabaseProviderBuilder
    {
        public static DbContextOptions<T> BuildInMemory<T>(string name) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseInMemoryDatabase(name);
            return builder.Options;
        }

        public static DbContextOptions<T> BuildSqlite<T>(string name) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite($"Filename=./Test.IdentityServer4.EntityFramework-3.1.0.{name}.db");
            return builder.Options;
        }

        public static DbContextOptions<T> BuildLocalDb<T>(string name) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(
                $@"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework-3.1.0.{name};trusted_connection=yes;");
            return builder.Options;
        }

        public static DbContextOptions<T> BuildAppVeyorSqlServer2016<T>(string name) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer($@"Server=(local)\SQL2016;Database=Test.IdentityServer4.EntityFramework-3.1.0.{name};User ID=sa;Password=Password12!");
            return builder.Options;
        }
    }
}
