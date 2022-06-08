﻿// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;
using ApiResource = IdentityServer4.Models.ApiResource;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ApiResourceMappersTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            ApiResourceMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ApiResourceMapperProfile>();
        }

        [Fact]
        public void Can_Map()
        {
            var model = new ApiResource();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }

        [Fact]
        public void Properties_Map()
        {
            var model = new ApiResource()
            {
               Description = "description",
               DisplayName = "displayname",
               Name = "foo",
               Scopes = { "foo1", "foo2" },
               Enabled = false
            };


            var mappedEntity = model.ToEntity();

            mappedEntity.Scopes.Count.Should().Be(2);
            var foo1 = mappedEntity.Scopes.FirstOrDefault(x => x.Scope == "foo1");
            foo1.Should().NotBeNull();
            var foo2 = mappedEntity.Scopes.FirstOrDefault(x => x.Scope == "foo2");
            foo2.Should().NotBeNull();
            

            var mappedModel = mappedEntity.ToModel();
            
            mappedModel.Description.Should().Be("description");
            mappedModel.DisplayName.Should().Be("displayname");
            mappedModel.Enabled.Should().BeFalse();
            mappedModel.Name.Should().Be("foo");
        }

        [Fact]
        public void missing_values_should_use_defaults()
        {
            var entity = new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Secrets = new System.Collections.Generic.List<Entities.ApiResourceSecret>
                {
                    new Entities.ApiResourceSecret
                    {
                    }
                }
            };

            var def = new ApiResource
            {
                ApiSecrets = { new Models.Secret("foo") }
            };

            var model = entity.ToModel();
            model.ApiSecrets.First().Type.Should().Be(def.ApiSecrets.First().Type);
        }
    }
}
