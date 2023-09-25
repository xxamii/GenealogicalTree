using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    internal class LogoutPageCommand : IPageCommand
    {
        private readonly ISession _session;

        public LogoutPageCommand(ISession session)
        {
            _session = session;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine();

            if (_session.IsUserLoggedIn)
            {
                _session.Logout();
                Console.WriteLine("You have successfully logged out");
            }
            else
            {
                Console.WriteLine("You are not logged in");
            }
        }

        public void ShowPage()
        {
            return;
        }
    }
}
