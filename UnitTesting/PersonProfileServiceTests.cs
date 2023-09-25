using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;
using Core.ViewModels;
using DAL.Abstractions.Interfaces;
using Moq;
using Xunit;

namespace UnitTesting
{
    public class PersonProfileServiceTests
    {
        [Fact]
        public async Task CreatePersonProfile_NotLoggedIn_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 1;
            PersonProfile testPersonProfile = new PersonProfile();

            session.Setup(a => a.IsUserLoggedIn).Returns(false);
            session.Setup(a => a.CurrentUser).Equals(null);

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_NotAdministrator_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 1;
            PersonProfile testPersonProfile = new PersonProfile();

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.User});

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_EmptyName_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 1;
            PersonProfile testPersonProfile = new PersonProfile {
                Birthday = DateTime.Now,
                Name = string.Empty,
                Country = "Test Country",
                City = "Test City"
            };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_EmptyCountry_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 1;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = string.Empty,
                City = "Test City"
            };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_EmptyCity_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 1;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = "Test Country",
                City = string.Empty
            };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_AllEmpty_DoesNotCreate()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 3;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = string.Empty,
                Country = string.Empty,
                City = string.Empty
            };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.Null(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_EmptyRelatives_CreatesNotRelatives()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 0;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = "Test Country",
                City = "Test City"
            };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });
            personProfileRepository.Setup(a => a.SerializeAsync(testPersonProfile))
                .ReturnsAsync(testPersonProfile);
            parentChildRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<ParentChild, bool>>()))
                .ReturnsAsync(new List<ParentChild>());

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile);

            Assert.NotNull(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_MoreThanTwoParents_CreatesWithoutParents()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 2;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = "Test Country",
                City = "Test City"
            };
            List<int> testParentsIds = new List<int> { 1, 2, 3, 4, 5 };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });
            personProfileRepository.Setup(a => a.SerializeAsync(testPersonProfile))
                .ReturnsAsync(testPersonProfile);
            parentChildRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<ParentChild, bool>>()))
                .ReturnsAsync(new List<ParentChild>());

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile, testParentsIds);

            Assert.NotNull(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_ParentDoesNotExist_CreatesWithoutParents()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 3;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = "Test Country",
                City = "Test City"
            };
            List<int> testParentsIds = new List<int> { 1, 2 };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });
            personProfileRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<PersonProfile, bool>>()))
                .ReturnsAsync(new List<PersonProfile>());
            personProfileRepository.Setup(a => a.SerializeAsync(testPersonProfile))
                .ReturnsAsync(testPersonProfile);
            parentChildRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<ParentChild, bool>>()))
                .ReturnsAsync(new List<ParentChild>());

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile, testParentsIds);

            Assert.NotNull(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task CreatePersonProfile_ChildDoesNotExist_CreatesWithoutChildren()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            int testLength = 3;
            PersonProfile testPersonProfile = new PersonProfile
            {
                Birthday = DateTime.Now,
                Name = "Test Name",
                Country = "Test Country",
                City = "Test City"
            };
            List<int> testChildrenIds = new List<int> { 1, 2 };

            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(new User { Role = Core.Role.Administrator });
            personProfileRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<PersonProfile, bool>>()))
                .ReturnsAsync(new List<PersonProfile>());
            personProfileRepository.Setup(a => a.SerializeAsync(testPersonProfile))
                .ReturnsAsync(testPersonProfile);
            parentChildRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<ParentChild, bool>>()))
                .ReturnsAsync(new List<ParentChild>());

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (PersonProfile?, List<string>) result = await personProfileService.CreatePersonProfileAsync(testPersonProfile, children: testChildrenIds);

            Assert.NotNull(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task DeletePersonProfileAsync_NotLoggedIn_DoesNotDelete()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            session.Setup(a => a.IsUserLoggedIn).Returns(false);
            session.Setup(a => a.CurrentUser).Equals(null);

            int testId = 2;
            int testLength = 1;

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (bool, List<string>) result = await personProfileService.DeletePersonProfileAsync(testId);

            Assert.False(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task DeletePersonProfileAsync_NotAdministrator_DoesNotDelete()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            User testUser = new User { Role = Core.Role.User };
            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(testUser);

            int testId = 2;
            int testLength = 1;

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (bool, List<string>) result = await personProfileService.DeletePersonProfileAsync(testId);

            Assert.False(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task DeletePersonProfileAsync_DoesNotExist_DoesNotDelete()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            User testUser = new User { Role = Core.Role.Administrator };
            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(testUser);

            personProfileRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<PersonProfile, bool>>()))
                .ReturnsAsync(new List<PersonProfile>());

            int testId = 2;
            int testLength = 1;

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (bool, List<string>) result = await personProfileService.DeletePersonProfileAsync(testId);

            Assert.False(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }

        [Fact]
        public async Task DeletePersonProfileAsync_Exists_Deletes()
        {
            var session = new Mock<ISession>();
            var personProfileRepository = new Mock<IGenericRepository<PersonProfile>>();
            var parentChildRepository = new Mock<IGenericRepository<ParentChild>>();

            User testUser = new User { Role = Core.Role.Administrator };
            session.Setup(a => a.IsUserLoggedIn).Returns(true);
            session.Setup(a => a.CurrentUser).Returns(testUser);

            int testId = 2;
            PersonProfile testPersonProfile = new PersonProfile { Id = testId };
            personProfileRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<PersonProfile, bool>>()))
                .ReturnsAsync(new List<PersonProfile> { testPersonProfile });

            parentChildRepository.Setup(a => a.DeserializeAsync(It.IsAny<Func<ParentChild, bool>>()))
                .ReturnsAsync(new List<ParentChild>());

            int testLength = 0;

            IPersonProfileService personProfileService = new PersonProfileService(session.Object,
                personProfileRepository.Object, parentChildRepository.Object);

            (bool, List<string>) result = await personProfileService.DeletePersonProfileAsync(testId);

            Assert.True(result.Item1);
            Assert.Equal(testLength, result.Item2.Count());
        }
    }
}
