using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Abstractions.Interfaces;
using Xunit;

namespace UnitTesting
{
    public class GenericRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _fixture;

        public GenericRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SerializeAsync_Serializes()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity testEntity = new TestEntity {
                Number = 138,
                Name = "Test",
                EnumValue = TestEnum.ThirdValue
            };

            TestEntity toSerialize = new TestEntity {
                Number = testEntity.Number,
                Name = testEntity.Name,
                EnumValue = testEntity.EnumValue
            };

            TestEntity result = await genericRepository.SerializeAsync(toSerialize);

            Assert.Equal(testEntity.Number, result.Number);
            Assert.Equal(testEntity.Name, result.Name);
            Assert.Equal(testEntity.EnumValue, result.EnumValue);
            Assert.NotEqual(testEntity.Id, result.Id);
        }

        [Fact]
        public async Task DeleteAsync_Exists_Deletes()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity entity = new TestEntity
            {
                Id = 0,
                Number = 2,
                Name = "ToDelete",
                EnumValue = TestEnum.SecondValue
            };

            bool result = await genericRepository.DeleteAsync(entity);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_DoesNotExist_DoesNotDelete()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity entity = new TestEntity
            {
                Id = 17,
                Number = 35,
                Name = "DoesNotExist",
                EnumValue = TestEnum.ThirdValue
            };

            bool result = await genericRepository.DeleteAsync(entity);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_Exists_Updates()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity entity = new TestEntity
            {
                Id = 1,
                Number = 35,
                Name = "Updated",
                EnumValue = TestEnum.ThirdValue
            };

            TestEntity? result = await genericRepository.UpdateAsync(entity);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotExist_DoesNotUpdate()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            TestEntity testEntity = new TestEntity
            {
                Id = 17,
                Number = 35,
                Name = "DoesNotExist",
                EnumValue = TestEnum.ThirdValue
            };

            TestEntity? result = await genericRepository.UpdateAsync(testEntity);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeserializeAsync_All()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            List<TestEntity?> testEntities = await genericRepository.DeserializeAsync();

            Assert.NotNull(testEntities);
            Assert.True(testEntities.Count() > 0);
        }

        [Fact]
        public async Task DeserializeAsync_ById()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            int testId = 2;
            string testName = "ToDeserialize";

            TestEntity? testEntity = (await genericRepository.DeserializeAsync(a => a.Id == testId)).FirstOrDefault();

            Assert.NotNull(testEntity);
            Assert.Equal(testName, testEntity.Name);
        }

        [Fact]
        public async Task DeserializeAsync_ById_DoesNotExist()
        {
            IGenericRepository<TestEntity> genericRepository = new GenericRepository<TestEntity>(_fixture.DatabasePath);

            int testId = 17;

            TestEntity? testEntity = (await genericRepository.DeserializeAsync(a => a.Id == testId)).FirstOrDefault();

            Assert.Null(testEntity);
        }
    }
}