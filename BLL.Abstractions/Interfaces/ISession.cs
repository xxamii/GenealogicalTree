using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface ISession
    {
        public bool IsUserLoggedIn { get; }

        public User? CurrentUser { get; }

        public Task<(bool, List<string>)> TryLoginAsync(string username, string password);

        public Task<(bool, List<string>)> TryRegisterAsync(string username, string password);

        public void Logout();
    }
}
