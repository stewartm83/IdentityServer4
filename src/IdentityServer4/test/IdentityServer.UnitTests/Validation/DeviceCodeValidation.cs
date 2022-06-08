// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer.UnitTests.Validation.Setup;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class DeviceCodeValidation
    {
        private const string Category = "Device code validation";

        private readonly IClientStore _clients = Factory.CreateClientStore();

        private readonly DeviceCode deviceCode = new DeviceCode
        {
            ClientId = "device_flow",
            IsAuthorized = true,
            Subject = new IdentityServerUser("bob").CreatePrincipal(),
            IsOpenId = true,
            Lifetime = 300,
            CreationTime = DateTime.UtcNow,
            AuthorizedScopes = new[] { "openid", "profile", "resource" }
        };

        [Fact]
        [Trait("Category", Category)]
        public async Task DeviceCode_Missing()
        {
            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = null, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task DeviceCode_From_Different_Client()
        {
            var badActor = await _clients.FindClientByIdAsync("codeclient");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(badActor);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Expired_DeviceCode()
        {
            deviceCode.CreationTime = DateTime.UtcNow.AddDays(-10);
            deviceCode.Lifetime = 300;

            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.ExpiredToken);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Access_Denied()
        {
            deviceCode.AuthorizedScopes = new List<string>();

            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.AccessDenied);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task DeviceCode_Not_Yet_Authorized()
        {
            deviceCode.IsAuthorized = false;

            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.AuthorizationPending);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task DeviceCode_Missing_Subject()
        {
            deviceCode.Subject = null;

            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.AuthorizationPending);
        }


        [Fact]
        [Trait("Category", Category)]
        public async Task User_Disabled()
        {
            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service, new TestProfileService(false));

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.InvalidGrant);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task DeviceCode_Polling_Too_Fast()
        {
            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service, throttlingService: new TestDeviceFlowThrottlingService(true));

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext { DeviceCode = handle, Request = request };

            await validator.ValidateAsync(context);

            context.Result.IsError.Should().BeTrue();
            context.Result.Error.Should().Be(OidcConstants.TokenErrors.SlowDown);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Valid_DeviceCode()
        {
            var client = await _clients.FindClientByIdAsync("device_flow");
            var service = Factory.CreateDeviceCodeService();

            var handle = await service.StoreDeviceAuthorizationAsync(Guid.NewGuid().ToString(), deviceCode);

            var validator = Factory.CreateDeviceCodeValidator(service);

            var request = new ValidatedTokenRequest();
            request.SetClient(client);

            var context = new DeviceCodeValidationContext {DeviceCode = handle, Request = request};

            await validator.ValidateAsync(context);
            
            context.Result.IsError.Should().BeFalse();
        }
    }
}
