// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace IdentityServer4.EntityFramework.Options
{
    /// <summary>
    /// Class to control a table's name and schema.
    /// </summary>
    public class TableConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TableConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        public TableConfiguration(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>
        /// The schema.
        /// </value>
        public string Schema { get; set; }
    }
}
