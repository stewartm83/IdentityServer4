// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ScopesMappersTests
    {
        [Fact]
        public void ScopeAutomapperConfigurationIsValid()
        {
            ScopeMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ScopeMapperProfile>();
        }

        [Fact]
        public void CanMapScope()
        {
            var model = new ApiScope();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }

        [Fact]
        public void Properties_Map()
        {
            var model = new ApiScope()
            {
                Description = "description",
                DisplayName = "displayname",
                Name = "foo",
                UserClaims = { "c1", "c2" },
                Properties = {
                    { "x", "xx" },
                    { "y", "yy" },
               },
                Enabled = false
            };


            var mappedEntity = model.ToEntity();
            mappedEntity.Description.Should().Be("description");
            mappedEntity.DisplayName.Should().Be("displayname");
            mappedEntity.Name.Should().Be("foo");

            mappedEntity.UserClaims.Count.Should().Be(2);
            mappedEntity.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(new[] { "c1", "c2" });
            mappedEntity.Properties.Count.Should().Be(2);
            mappedEntity.Properties.Should().Contain(x => x.Key == "x" && x.Value == "xx");
            mappedEntity.Properties.Should().Contain(x => x.Key == "y" && x.Value == "yy");


            var mappedModel = mappedEntity.ToModel();

            mappedModel.Description.Should().Be("description");
            mappedModel.DisplayName.Should().Be("displayname");
            mappedModel.Enabled.Should().BeFalse();
            mappedModel.Name.Should().Be("foo");
            mappedModel.UserClaims.Count.Should().Be(2);
            mappedModel.UserClaims.Should().BeEquivalentTo(new[] { "c1", "c2" });
            mappedModel.Properties.Count.Should().Be(2);
            mappedModel.Properties["x"].Should().Be("xx");
            mappedModel.Properties["y"].Should().Be("yy");
        }
    }
}
