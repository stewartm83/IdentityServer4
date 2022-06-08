// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Services.Default;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling
{
    public class DeviceAuthorizationResponseGeneratorTests
    {
        private readonly List<IdentityResource> identityResources = new List<IdentityResource> { new IdentityResources.OpenId(), new IdentityResources.Profile() };
        private readonly List<ApiResource> apiResources = new List<ApiResource> { new ApiResource("resource") { Scopes = { "api1" } } };
        private readonly List<ApiScope> scopes = new List<ApiScope> { new ApiScope("api1") };

        private readonly FakeUserCodeGenerator fakeUserCodeGenerator = new FakeUserCodeGenerator();
        private readonly IDeviceFlowCodeService deviceFlowCodeService = new DefaultDeviceFlowCodeService(new InMemoryDeviceFlowStore(), new StubHandleGenerationService());
        private readonly IdentityServerOptions options = new IdentityServerOptions();
        private readonly StubClock clock = new StubClock();

        private readonly DeviceAuthorizationResponseGenerator generator;
        private readonly DeviceAuthorizationRequestValidationResult testResult;
        private const string TestBaseUrl = "http://localhost:5000/";

        public DeviceAuthorizationResponseGeneratorTests()
        {
            testResult = new DeviceAuthorizationRequestValidationResult(new ValidatedDeviceAuthorizationRequest
            {
                Client = new Client { ClientId = Guid.NewGuid().ToString() },
                IsOpenIdRequest = true,
                ValidatedResources = new ResourceValidationResult()
            });

            generator = new DeviceAuthorizationResponseGenerator(
                options,
                new DefaultUserCodeService(new IUserCodeGenerator[] { new NumericUserCodeGenerator(), fakeUserCodeGenerator }),
                deviceFlowCodeService,
                clock,
                new NullLogger<DeviceAuthorizationResponseGenerator>());
        }

        [Fact]
        public async Task ProcessAsync_when_valiationresult_null_exect_exception()
        {
            Func<Task> act = () => generator.ProcessAsync(null, TestBaseUrl);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessAsync_when_valiationresult_client_null_exect_exception()
        {
            var validationResult = new DeviceAuthorizationRequestValidationResult(new ValidatedDeviceAuthorizationRequest());
            Func<Task> act = () => generator.ProcessAsync(validationResult, TestBaseUrl);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessAsync_when_baseurl_null_exect_exception()
        {
            Func<Task> act = () => generator.ProcessAsync(testResult, null);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ProcessAsync_when_user_code_collision_expect_retry()
        {
            var creationTime = DateTime.UtcNow;
            clock.UtcNowFunc = () => creationTime;

            testResult.ValidatedRequest.Client.UserCodeType = FakeUserCodeGenerator.UserCodeTypeValue;
            await deviceFlowCodeService.StoreDeviceAuthorizationAsync(FakeUserCodeGenerator.TestCollisionUserCode, new DeviceCode());

            var response = await generator.ProcessAsync(testResult, TestBaseUrl);

            response.UserCode.Should().Be(FakeUserCodeGenerator.TestUniqueUserCode);
        }

        [Fact]
        public async Task ProcessAsync_when_user_code_collision_retry_limit_reached_expect_error()
        {
            var creationTime = DateTime.UtcNow;
            clock.UtcNowFunc = () => creationTime;

            fakeUserCodeGenerator.RetryLimit = 1;
            testResult.ValidatedRequest.Client.UserCodeType = FakeUserCodeGenerator.UserCodeTypeValue;
            await deviceFlowCodeService.StoreDeviceAuthorizationAsync(FakeUserCodeGenerator.TestCollisionUserCode, new DeviceCode());

            await Assert.ThrowsAsync<InvalidOperationException>(() => generator.ProcessAsync(testResult, TestBaseUrl));
        }

        [Fact]
        public async Task ProcessAsync_when_generated_expect_user_code_stored()
        {
            var creationTime = DateTime.UtcNow;
            clock.UtcNowFunc = () => creationTime;

            testResult.ValidatedRequest.RequestedScopes = new List<string> { "openid", "api1" };
            testResult.ValidatedRequest.ValidatedResources = new ResourceValidationResult(new Resources(
                identityResources.Where(x => x.Name == "openid"),
                apiResources.Where(x => x.Name == "resource"),
                scopes.Where(x => x.Name == "api1")));

            var response = await generator.ProcessAsync(testResult, TestBaseUrl);

            response.UserCode.Should().NotBeNullOrWhiteSpace();

            var userCode = await deviceFlowCodeService.FindByUserCodeAsync(response.UserCode);
            userCode.Should().NotBeNull();
            userCode.ClientId.Should().Be(testResult.ValidatedRequest.Client.ClientId);
            userCode.Lifetime.Should().Be(testResult.ValidatedRequest.Client.DeviceCodeLifetime);
            userCode.CreationTime.Should().Be(creationTime);
            userCode.Subject.Should().BeNull();
            userCode.AuthorizedScopes.Should().BeNull();

            userCode.RequestedScopes.Should().Contain(testResult.ValidatedRequest.RequestedScopes);
        }

        [Fact]
        public async Task ProcessAsync_when_generated_expect_device_code_stored()
        {
            var creationTime = DateTime.UtcNow;
            clock.UtcNowFunc = () => creationTime;

            var response = await generator.ProcessAsync(testResult, TestBaseUrl);

            response.DeviceCode.Should().NotBeNullOrWhiteSpace();
            response.Interval.Should().Be(options.DeviceFlow.Interval);

            var deviceCode = await deviceFlowCodeService.FindByDeviceCodeAsync(response.DeviceCode);
            deviceCode.Should().NotBeNull();
            deviceCode.ClientId.Should().Be(testResult.ValidatedRequest.Client.ClientId);
            deviceCode.IsOpenId.Should().Be(testResult.ValidatedRequest.IsOpenIdRequest);
            deviceCode.Lifetime.Should().Be(testResult.ValidatedRequest.Client.DeviceCodeLifetime);
            deviceCode.CreationTime.Should().Be(creationTime);
            deviceCode.Subject.Should().BeNull();
            deviceCode.AuthorizedScopes.Should().BeNull();

            response.DeviceCodeLifetime.Should().Be(deviceCode.Lifetime);
        }

        [Fact]
        public async Task ProcessAsync_when_DeviceVerificationUrl_is_relative_uri_expect_correct_VerificationUris()
        {
            const string baseUrl = "http://localhost:5000/";
            options.UserInteraction.DeviceVerificationUrl = "/device";
            options.UserInteraction.DeviceVerificationUserCodeParameter = "userCode";

            var response = await generator.ProcessAsync(testResult, baseUrl);

            response.VerificationUri.Should().Be("http://localhost:5000/device");
            response.VerificationUriComplete.Should().StartWith("http://localhost:5000/device?userCode=");
        }

        [Fact]
        public async Task ProcessAsync_when_DeviceVerificationUrl_is_absolute_uri_expect_correct_VerificationUris()
        {
            const string baseUrl = "http://localhost:5000/";
            options.UserInteraction.DeviceVerificationUrl = "http://short/device";
            options.UserInteraction.DeviceVerificationUserCodeParameter = "userCode";

            var response = await generator.ProcessAsync(testResult, baseUrl);

            response.VerificationUri.Should().Be("http://short/device");
            response.VerificationUriComplete.Should().StartWith("http://short/device?userCode=");
        }
    }

    internal class FakeUserCodeGenerator : IUserCodeGenerator
    {
        public const string UserCodeTypeValue = "Collider";
        public const string TestUniqueUserCode = "123";
        public const string TestCollisionUserCode = "321";
        private int _tryCount = 0;
        private int _retryLimit = 2;


        public string UserCodeType => UserCodeTypeValue;

        public int RetryLimit
        {
            get => _retryLimit;
            set => _retryLimit = value;
        }

        public Task<string> GenerateAsync()
        {
            if (_tryCount == 0)
            {
                _tryCount++;
                return Task.FromResult(TestCollisionUserCode);
            }

            _tryCount++;
            return Task.FromResult(TestUniqueUserCode);
        }
    }
}
