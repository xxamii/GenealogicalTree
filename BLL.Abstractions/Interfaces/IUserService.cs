using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetUsersAsync(Func<User, bool> predicate = null);

        public Task<(User?, List<string>)> TryRegisterAsync(string username, string password);

        public Task<(User?, List<string>)> TryLoginAsync(string username, string password);
    }
}
