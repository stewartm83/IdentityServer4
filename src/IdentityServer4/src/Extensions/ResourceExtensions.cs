// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Extensions;
using IdentityServer4.Validation;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Extensions for Resource
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// Returns the collection of scope values that are required.
        /// </summary>
        /// <param name="resourceValidationResult"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRequiredScopeValues(this ResourceValidationResult resourceValidationResult)
        {
            var names = resourceValidationResult.Resources.IdentityResources.Where(x => x.Required).Select(x => x.Name).ToList();
            names.AddRange(resourceValidationResult.Resources.ApiScopes.Where(x => x.Required).Select(x => x.Name));

            var values = resourceValidationResult.ParsedScopes.Where(x => names.Contains(x.ParsedName)).Select(x => x.RawValue);
            return values;
        }

        /// <summary>
        /// Converts to scope names.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <returns></returns>
        public static IEnumerable<string> ToScopeNames(this Resources resources)
        {
            var names = resources.IdentityResources.Select(x => x.Name).ToList();
            names.AddRange(resources.ApiScopes.Select(x => x.Name));
            if (resources.OfflineAccess)
            {
                names.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            }
            
            return names;
        }

        /// <summary>
        /// Finds the IdentityResource that matches the scope.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IdentityResource FindIdentityResourcesByScope(this Resources resources, string name)
        {
            var q = from id in resources.IdentityResources
                    where id.Name == name
                    select id;
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Finds the API resources that contain the scope.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IEnumerable<ApiResource> FindApiResourcesByScope(this Resources resources, string name)
        {
            var q = from api in resources.ApiResources
                    where api.Scopes != null && api.Scopes.Contains(name)
                    select api;
            return q.ToArray();
        }

        /// <summary>
        /// Finds the API scope.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ApiScope FindApiScope(this Resources resources, string name)
        {
            var q = from scope in resources.ApiScopes
                    where scope.Name == name
                    select scope;
            return q.FirstOrDefault();
        }

        internal static Resources FilterEnabled(this Resources resources)
        {
            if (resources == null) return new Resources();

            return new Resources(
                resources.IdentityResources.Where(x => x.Enabled),
                resources.ApiResources.Where(x => x.Enabled),
                resources.ApiScopes.Where(x => x.Enabled))
            {
                OfflineAccess = resources.OfflineAccess
            };
        }

        internal static ICollection<string> FindMatchingSigningAlgorithms(this IEnumerable<ApiResource> apiResources)
        {
            var apis = apiResources.ToList();

            if (apis.IsNullOrEmpty())
            {
                return new List<string>();
            }

            // only one API resource request, forward the allowed signing algorithms (if any)
            if (apis.Count == 1)
            {
                return apis.First().AllowedAccessTokenSigningAlgorithms;
            }
            
            var allAlgorithms = apis.Where(r => r.AllowedAccessTokenSigningAlgorithms.Any()).Select(r => r.AllowedAccessTokenSigningAlgorithms).ToList();

            // resources need to agree on allowed signing algorithms
            if (allAlgorithms.Any())
            {
                var allowedAlgorithms = IntersectLists(allAlgorithms);

                if (allowedAlgorithms.Any())
                {
                    return allowedAlgorithms.ToHashSet();
                }

                throw new InvalidOperationException("Signing algorithms requirements for requested resources are not compatible.");
            }

            return new List<string>();
        }

        private static IEnumerable<T> IntersectLists<T>(IEnumerable<IEnumerable<T>> lists)
        {
            return lists.Aggregate((l1, l2) => l1.Intersect(l2));
        }
    }
}
