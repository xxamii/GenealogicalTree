using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsersAsync(Func<User, bool> predicate = null)
        {
            return await _userRepository.DeserializeAsync(predicate);
        }

        public async Task<(User?, List<string>)> TryRegisterAsync(string username, string password)
        {
            User toRegister = new User { Username = username, Password = password };
            (User?, List<string>) result = (null, new List<string>());
            User? testUser = await GetUserByName(toRegister);

            if (testUser != null)
            {
                result.Item2.Add($"Username {toRegister.Username} is already taken");
                return result;
            }

            (bool, List<string>) isPasswordValid = IsPasswordValid(toRegister.Password);
            if (isPasswordValid.Item1)
            {
                User user = await _userRepository.SerializeAsync(toRegister);
                result.Item1 = user;
            }
            else
            {
                result.Item2.AddRange(isPasswordValid.Item2);
            }

            return result;
        }

        public async Task<(User?, List<string>)> TryLoginAsync(string username, string password)
        {
            User toLogin = new User { Username = username, Password = password };
            (User?, List<string>) result = (null, new List<string>());

            User? user = await GetUserByName(toLogin);

            if (user != null && user.Password == toLogin.Password)
            {
                result.Item1 = user;
            }
            else
            {
                result.Item2.Add("Wrong password or username");
            }

            return result;
        }

        private async Task<User?> GetUserByName(User user)
        {
            List<User> users = await GetUsersAsync(a => a.Username == user.Username);

            return users.FirstOrDefault();
        }

        private (bool, List<string>) IsPasswordValid(string password)
        {
            (bool, List<string>) result = (true, new List<string>());

            if (password.Length < 8)
            {
                result.Item1 = false;
                result.Item2.Add("Password should be at least 8 characters long");
            }

            return result;
        }
    }
}
