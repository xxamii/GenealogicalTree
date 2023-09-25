using BLL.Abstractions.Interfaces;
using Core;
using Core.Models;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    public class CreatePersonProfilePageCommand : IPageCommand
    {
        private readonly ISession _session;
        private readonly IPersonProfileService _personProfileService;

        public CreatePersonProfilePageCommand(ISession session, IPersonProfileService personProfileService)
        {
            _session = session;
            _personProfileService = personProfileService;
        }

        public async Task ExecuteAsync()
        {
            if (_session.IsUserLoggedIn && _session.CurrentUser != null && _session.CurrentUser.Role == Role.Administrator)
            {
                Console.Write("Full name: ");
                string name = ConsoleExtensions.ReadNonEmpty(Console.ReadLine());

                Console.Write("Country: ");
                string country = ConsoleExtensions.ReadNonEmpty(Console.ReadLine()).Capitalize();

                Console.Write("City: ");
                string city = ConsoleExtensions.ReadNonEmpty(Console.ReadLine()).Capitalize();

                Console.Write("Birthday (only dd.mm.yyyy): ");
                DateTime birthday = ConsoleExtensions.ReadDate(Console.ReadLine());

                Console.Write("Parents (sequence of ids, any separator): ");
                List<int> parents = ConsoleExtensions.ReadIntList(Console.ReadLine());

                Console.Write("Children (sequence of ids, any separator): ");
                List<int> children = ConsoleExtensions.ReadIntList(Console.ReadLine());

                PersonProfile personProfile = new PersonProfile
                {
                    Name = name,
                    Country = country,
                    City = city,
                    Birthday = birthday
                };

                (PersonProfile?, List<string>) result = await _personProfileService.CreatePersonProfileAsync(personProfile, parents, children);

                Console.WriteLine();

                if (result.Item1 != null)
                {
                    Console.WriteLine($"({result.Item1.Id}){result.Item1.Name} profile has been successfully created");
                }
                
                foreach(string message in result.Item2)
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
