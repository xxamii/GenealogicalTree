using BLL.Abstractions.Interfaces;
using Core.Models;

namespace BLL
{
    public class Session : ISession
    {
        public virtual bool IsUserLoggedIn {
            get
            {
                return CurrentUser != null;
            }
        }
        public virtual User? CurrentUser { get; private set; } = null;

        private readonly IUserService _userService;

        public Session(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<(bool, List<string>)> TryLoginAsync(string username, string password)
        {
            (User?, List<string>) result = await _userService.TryLoginAsync(username, password);

            if (result.Item1 != null)
            {
                CurrentUser = result.Item1;
            }

            return (result.Item1 != null, result.Item2);
        }

        public async Task<(bool, List<string>)> TryRegisterAsync(string username, string password)
        {
            (User?, List<string>) registrationResult = await _userService.TryRegisterAsync(username, password);

            if (registrationResult.Item1 != null)
            {
                await TryLoginAsync(registrationResult.Item1.Username, registrationResult.Item1.Password);
            }

            return (registrationResult.Item1 != null, registrationResult.Item2);
        }

        public void Logout()
        {
            if (IsUserLoggedIn)
            {
                CurrentUser = null;
            }
        }
    }
}
