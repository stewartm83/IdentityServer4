// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework
{
    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly OperationalStoreOptions _options;
        private readonly IPersistedGrantDbContext _persistedGrantDbContext;
        private readonly IOperationalStoreNotification _operationalStoreNotification;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="persistedGrantDbContext"></param>
        /// <param name="operationalStoreNotification"></param>
        /// <param name="logger"></param>
        public TokenCleanupService(
            OperationalStoreOptions options,
            IPersistedGrantDbContext persistedGrantDbContext, 
            ILogger<TokenCleanupService> logger,
            IOperationalStoreNotification operationalStoreNotification = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _persistedGrantDbContext = persistedGrantDbContext ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operationalStoreNotification = operationalStoreNotification;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            var found = Int32.MaxValue;
            
            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredGrants = await _persistedGrantDbContext.PersistedGrants
                    .Where(x => x.Expiration < DateTime.UtcNow)
                    .OrderBy(x => x.Expiration)
                    .Take(_options.TokenCleanupBatchSize)
                    .ToArrayAsync();

                found = expiredGrants.Length;
                _logger.LogInformation("Removing {grantCount} grants", found);

                if (found > 0)
                {
                    _persistedGrantDbContext.PersistedGrants.RemoveRange(expiredGrants);
                    await SaveChangesAsync();

                    if (_operationalStoreNotification != null)
                    {
                        await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants);
                    }
                }
            }
        }


        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            var found = Int32.MaxValue;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredCodes = await _persistedGrantDbContext.DeviceFlowCodes
                    .Where(x => x.Expiration < DateTime.UtcNow)
                    .OrderBy(x => x.Expiration)
                    .Take(_options.TokenCleanupBatchSize)
                    .ToArrayAsync();

                found = expiredCodes.Length;
                _logger.LogInformation("Removing {deviceCodeCount} device flow codes", found);

                if (found > 0)
                {
                    _persistedGrantDbContext.DeviceFlowCodes.RemoveRange(expiredCodes);
                    await SaveChangesAsync();

                    if (_operationalStoreNotification != null)
                    {
                        await _operationalStoreNotification.DeviceCodesRemovedAsync(expiredCodes);
                    }
                }
            }
        }

        private async Task SaveChangesAsync()
        {
            var count = 3;

            while (count > 0)
            {
                try
                {
                    await _persistedGrantDbContext.SaveChangesAsync();
                    return;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    count--;

                    // we get this if/when someone else already deleted the records
                    // we want to essentially ignore this, and keep working
                    _logger.LogDebug("Concurrency exception removing expired grants: {exception}", ex.Message);

                    foreach (var entry in ex.Entries)
                    {
                        // mark this entry as not attached anymore so we don't try to re-delete
                        entry.State = EntityState.Detached;
                    }
                }
            }

            _logger.LogDebug("Too many concurrency exceptions. Exiting.");
        }
    }
}
