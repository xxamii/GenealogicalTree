using BLL.Abstractions.Interfaces;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    internal class LoginPageCommand : IPageCommand
    {
        private readonly ISession _session;

        public LoginPageCommand(ISession session)
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

            (bool, List<string>) result = await _session.TryLoginAsync(username, password);

            Console.WriteLine();

            if (result.Item1)
            {
                Console.WriteLine("You have successfully logged in");
            }
            else
            {
                Console.WriteLine("Could not login");
            }

            foreach (string message in result.Item2)
            {
                Console.WriteLine(message);
            }
        }

        public void ShowPage()
        {
            Console.WriteLine("Login form\n");
        }
    }
}
