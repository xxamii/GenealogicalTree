using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    internal class RegisterPageCommand : IPageCommand
    {
        private readonly ISession _session;

        public RegisterPageCommand(ISession session)
        {
            _session = session;
        }

        public async Task ExecuteAsync()
        {
            ShowPage();

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            (bool, List<string>) registrationResult = await _session.TryRegisterAsync(username, password);

            Console.WriteLine();

            if (registrationResult.Item1)
            {
                Console.WriteLine("You have successfully registered");
            }
            else
            {
                Console.WriteLine("Could not register");
            }

            foreach (string message in registrationResult.Item2)
            {
                Console.WriteLine(message);
            }
        }

        public void ShowPage()
        {
            Console.WriteLine("Registration form\n");
        }
    }
}
