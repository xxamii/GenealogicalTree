using BLL.Abstractions.Interfaces;
using Core;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    public class DeletePersonProfilePageCommand : IPageCommand
    {
        private readonly IPersonProfileService _personProfileService;
        private readonly ISession _session;

        public DeletePersonProfilePageCommand(ISession session, IPersonProfileService personProfileService)
        {
            _personProfileService = personProfileService;
            _session = session;
        }

        public async Task ExecuteAsync()
        {
            if (_session.IsUserLoggedIn && _session.CurrentUser.Role == Role.Administrator)
            {
                Console.Write("Person's id: ");
                int id = ConsoleExtensions.ReadInt(Console.ReadLine());

                (bool, List<string>) deletionResult = await _personProfileService.DeletePersonProfileAsync(id);

                Console.WriteLine();

                if (deletionResult.Item1)
                {
                    Console.WriteLine("Deletion successful");
                }
                else
                {
                    Console.WriteLine("Could not delete");
                }

                foreach (string message in deletionResult.Item2)
                {
                    Console.WriteLine(message);
                }
            }
        }

        public void ShowPage()
        {
            throw new NotImplementedException();
        }
    }
}
