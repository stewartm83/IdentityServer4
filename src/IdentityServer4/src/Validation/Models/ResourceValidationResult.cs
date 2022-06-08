// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Result of validation of requested scopes and resource indicators.
    /// </summary>
    public class ResourceValidationResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ResourceValidationResult()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resources"></param>
        public ResourceValidationResult(Resources resources)
        {
            Resources = resources;
            ParsedScopes = resources.ToScopeNames().Select(x => new ParsedScopeValue(x)).ToList();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="parsedScopeValues"></param>
        public ResourceValidationResult(Resources resources, IEnumerable<ParsedScopeValue> parsedScopeValues)
        {
            Resources = resources;
            ParsedScopes = parsedScopeValues.ToList();
        }

        /// <summary>
        /// Indicates if the result was successful.
        /// </summary>
        public bool Succeeded => ParsedScopes.Any() && !InvalidScopes.Any();

        /// <summary>
        /// The resources of the result.
        /// </summary>
        public Resources Resources { get; set; } = new Resources();

        /// <summary>
        /// The parsed scopes represented by the result.
        /// </summary>
        public ICollection<ParsedScopeValue> ParsedScopes { get; set; } = new HashSet<ParsedScopeValue>();

        /// <summary>
        /// The original (raw) scope values represented by the validated result.
        /// </summary>
        public IEnumerable<string> RawScopeValues => ParsedScopes.Select(x => x.RawValue);

        /// <summary>
        /// The requested scopes that are invalid.
        /// </summary>
        public ICollection<string> InvalidScopes { get; set; } = new HashSet<string>();

        /// <summary>
        /// Returns new result filted by the scope values.
        /// </summary>
        /// <param name="scopeValues"></param>
        /// <returns></returns>
        public ResourceValidationResult Filter(IEnumerable<string> scopeValues)
        {
            scopeValues ??= Enumerable.Empty<string>();

            var offline = scopeValues.Contains(IdentityServerConstants.StandardScopes.OfflineAccess);

            var parsedScopesToKeep = ParsedScopes.Where(x => scopeValues.Contains(x.RawValue)).ToArray();
            var parsedScopeNamesToKeep = parsedScopesToKeep.Select(x => x.ParsedName).ToArray();

            var identityToKeep = Resources.IdentityResources.Where(x => parsedScopeNamesToKeep.Contains(x.Name));
            var apiScopesToKeep = Resources.ApiScopes.Where(x => parsedScopeNamesToKeep.Contains(x.Name));

            var apiScopesNamesToKeep = apiScopesToKeep.Select(x => x.Name).ToArray();
            var apiResourcesToKeep = Resources.ApiResources.Where(x => x.Scopes.Any(y => apiScopesNamesToKeep.Contains(y)));

            var resources = new Resources(identityToKeep, apiResourcesToKeep, apiScopesToKeep)
            {
                OfflineAccess = offline
            };
            
            return new ResourceValidationResult()
            {
                Resources = resources,
                ParsedScopes = parsedScopesToKeep
            };
        }
    }
}
