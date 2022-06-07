// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityServer4.Services
{
    /// <summary>
    /// The default device flow throttling service using IDistributedCache.
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IDeviceFlowThrottlingService" />
    public class DistributedDeviceFlowThrottlingService : IDeviceFlowThrottlingService
    {
        private readonly IDistributedCache _cache;
        private readonly ISystemClock _clock;
        private readonly IdentityServerOptions _options;

        private const string KeyPrefix = "devicecode_";

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedDeviceFlowThrottlingService"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="options">The options.</param>
        public DistributedDeviceFlowThrottlingService(
            IDistributedCache cache,
            ISystemClock clock,
            IdentityServerOptions options)
        {
            _cache = cache;
            _clock = clock;
            _options = options;
        }

        /// <summary>
        /// Decides if the requesting client and device code needs to slow down.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="details">The device code details.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceCode</exception>
        public async Task<bool> ShouldSlowDown(string deviceCode, DeviceCode details)
        {
            if (deviceCode == null) throw new ArgumentNullException(nameof(deviceCode));
            
            var key = KeyPrefix + deviceCode;
            var options = new DistributedCacheEntryOptions {AbsoluteExpiration = _clock.UtcNow.AddSeconds(details.Lifetime)};

            var lastSeenAsString = await _cache.GetStringAsync(key);

            // record new
            if (lastSeenAsString == null)
            {
                await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);
                return false;
            }

            // check interval
            if (DateTime.TryParse(lastSeenAsString, out var lastSeen))
            {
                if (_clock.UtcNow < lastSeen.AddSeconds(_options.DeviceFlow.Interval))
                {
                    await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);
                    return true;
                }
            }

            // store current and continue
            await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);
            return false;
        }
    }
}
