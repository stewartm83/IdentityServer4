// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// In-memory device flow store
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IDeviceFlowStore" />
    public class InMemoryDeviceFlowStore : IDeviceFlowStore
    {
        private readonly List<InMemoryDeviceAuthorization> _repository = new List<InMemoryDeviceAuthorization>();

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            lock (_repository)
            {
                _repository.Add(new InMemoryDeviceAuthorization(deviceCode, userCode, data));
            }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            DeviceCode foundDeviceCode;

            lock (_repository)
            {
                foundDeviceCode = _repository.FirstOrDefault(x => x.UserCode == userCode)?.Data;
            }

            return Task.FromResult(foundDeviceCode);
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            DeviceCode foundDeviceCode;

            lock (_repository)
            {
                foundDeviceCode = _repository.FirstOrDefault(x => x.DeviceCode == deviceCode)?.Data;
            }

            return Task.FromResult(foundDeviceCode);
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            lock (_repository)
            {
                var foundData = _repository.FirstOrDefault(x => x.UserCode == userCode);

                if (foundData != null)
                {
                    foundData.Data = data;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            lock (_repository)
            {
                var foundData = _repository.FirstOrDefault(x => x.DeviceCode == deviceCode);

                if (foundData != null)
                {
                    _repository.Remove(foundData);
                }
            }


            return Task.CompletedTask;
        }

        private class InMemoryDeviceAuthorization
        {
            public InMemoryDeviceAuthorization(string deviceCode, string userCode, DeviceCode data)
            {
                DeviceCode = deviceCode;
                UserCode = userCode;
                Data = data;
            }

            public string DeviceCode { get; }
            public string UserCode { get; }
            public DeviceCode Data { get; set; }
        }
    }
}
