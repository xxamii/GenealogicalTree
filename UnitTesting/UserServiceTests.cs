using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using BLL.Abstractions.Interfaces;
using DAL.Abstractions.Interfaces;
using Core.Models;
using Moq;
using Xunit;

namespace UnitTesting
{
    public class UserServiceTests
    {
        [Fact]
        public async Task TryRegisterAsync_AllValid()
        {
            User testUser = new User { Username = "Test", Password = "12345678" };
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.SerializeAsync(It.IsAny<User>())).ReturnsAsync(testUser);
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User>());
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 0;

            (User?, List<string>) result = await userService.TryRegisterAsync(testUser.Username, testUser.Password);

            Assert.NotNull(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
        }

        [Fact]
        public async Task TryRegisterAsync_UserExists_DoesNotRegister()
        {
            User testUser = new User { Username = "Test", Password = "12345678" };
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User> { testUser });
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 1;
            string resultMessage = "Username Test is already taken";

            (User?, List<string>) result = await userService.TryRegisterAsync(testUser.Username, testUser.Password);

            Assert.Null(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
            Assert.Equal(resultMessage, result.Item2[0]);
        }

        [Fact]
        public async Task TryRegisterAsync_InvalidPassword_DoesNotRegister()
        {
            User testUser = new User { Username = "Test", Password = "1" };
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User>());
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 1;
            string resultMessage = "Password should be at least 8 characters long";

            (User?, List<string>) result = await userService.TryRegisterAsync(testUser.Username, testUser.Password);

            Assert.Null(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
            Assert.Equal(resultMessage, result.Item2[0]);
        }

        [Fact]
        public async Task TryLoginAsync_AllValid()
        {
            User testUser = new User { Username = "Test", Password = "12345678" };
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User> { testUser });
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 0;

            (User?, List<string>) result = await userService.TryLoginAsync(testUser.Username, testUser.Password);

            Assert.NotNull(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
        }

        [Fact]
        public async Task TryLoginAsync_UserDoesNotExist_DoesNotLogin()
        {
            User testUser = new User { Username = "Test", Password = "12345678" };
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User>());
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 1;
            string resultMessage = "Wrong password or username";

            (User?, List<string>) result = await userService.TryLoginAsync(testUser.Username, testUser.Password);

            Assert.Null(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
            Assert.Equal(resultMessage, result.Item2[0]);
        }

        [Fact]
        public async Task TryLoginAsync_WrongPassword_DoesNotLogin()
        {
            User testUser = new User { Username = "Test", Password = "12345678" };
            string testPassword = "1";
            var userRepository = new Mock<IGenericRepository<User>>();
            userRepository.Setup(x => x.DeserializeAsync(It.IsAny<Func<User, bool>>())).ReturnsAsync(new List<User> { testUser });
            IUserService userService = new UserService(userRepository.Object);
            int messageCount = 1;
            string resultMessage = "Wrong password or username";

            (User?, List<string>) result = await userService.TryLoginAsync(testUser.Username, testPassword);

            Assert.Null(result.Item1);
            Assert.Equal(messageCount, result.Item2.Count());
            Assert.Equal(resultMessage, result.Item2[0]);
        }
    }
}
