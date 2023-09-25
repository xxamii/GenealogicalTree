using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Abstractions.Interfaces;
using Xunit;

namespace Testing
{
    public class GenericRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _fixture;

        public GenericRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SerializeAsync_entity_does_not_exist()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity testEntity = new TestEntity {
                Number = 138,
                Name = "Test",
                EnumValue = TestEnum.ThirdValue,
                Values = new List<int> { 1, 2, 3, 4, 5 }
            };

            TestEntity result = await genericRepository.SerializeAsync(testEntity);

            Assert.Equal(testEntity.Number, result.Number);
            Assert.Equal(testEntity.Name, result.Name);
            Assert.Equal(testEntity.EnumValue, result.EnumValue);
            Assert.NotEqual(testEntity.Id, result.Id);
        }

        [Fact]
        public async Task DeleteAsync_entity_exists()
        {

        }

        [Fact]
        public async Task DeleteAsync_entity_does_not_exist()
        {

        }

        [Fact]
        public async Task UpdateAsync_entity_exists()
        {

        }

        [Fact]
        public async Task UpdateAsync_entity_does_not_exist()
        {

        }

        [Fact]
        public async Task DeserializeAsync_entity_exists()
        {

        }

        [Fact]
        public async Task DeserializeAsync_entity_does_not_exist()
        {

        }
    }
}