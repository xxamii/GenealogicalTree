using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using BLL.Abstractions.Interfaces;
using Core;
using Core.Models;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    public class UpdatePersonProfilePageCommand : IPageCommand
    {
        private readonly ISession _session;
        private readonly IPersonProfileService _personProfileService;

        public UpdatePersonProfilePageCommand(ISession session, IPersonProfileService personProfileService)
        {
            _session = session;
            _personProfileService = personProfileService;
        }

        public async Task ExecuteAsync()
        {
            if (_session.IsUserLoggedIn && _session.CurrentUser.Role == Role.Administrator)
            {
                Console.Write("Person's id: ");
                int id = ConsoleExtensions.ReadInt(Console.ReadLine());
                Console.WriteLine();

                PersonProfile? personProfile = (await _personProfileService.GetPersonProfilesAsync(a => a.Id == id)).FirstOrDefault();

                if (personProfile == null)
                {
                    Console.WriteLine($"Could not find a person with id {id}");
                    return;
                }

                Console.Write("Full name (leave empty to not change): ");
                string name = Console.ReadLine();

                if (name != null && name.Trim().Length > 0)
                {
                    personProfile.Name = name;
                }

                Console.Write("Country (leave empty to not change): ");
                string country = Console.ReadLine().Capitalize();

                if (country != null && country.Trim().Length > 0)
                {
                    personProfile.Country = country;
                }

                Console.Write("City (leave empty to not change): ");
                string city = Console.ReadLine().Capitalize();

                if (city != null && city.Trim().Length > 0)
                {
                    personProfile.City = city;
                }

                Console.Write("Change birthday date? (y or n): ");
                string check = ConsoleExtensions.ReadSpecificString(Console.ReadLine().ToLower(),
                    new List<string> { "y", "n" });

                if (check.ToLower() == "y")
                {
                    Console.Write("Birthday: ");
                    DateTime birthday = ConsoleExtensions.ReadDate(Console.ReadLine());
                    personProfile.Birthday = birthday;
                }

                Console.Write("Change parents? (y or n): ");
                check = ConsoleExtensions.ReadSpecificString(Console.ReadLine().ToLower(),
                    new List<string> { "y", "n" });

                List<int>? parents = null;

                if (check.ToLower() == "y")
                {
                    Console.Write("Parents (sequence of ids, any separator): ");
                    parents = ConsoleExtensions.ReadIntList(Console.ReadLine());
                }

                Console.Write("Change children? (y or n): ");
                check = ConsoleExtensions.ReadSpecificString(Console.ReadLine().ToLower(),
                    new List<string> { "y", "n" });

                List<int>? children = null;

                if (check.ToLower() == "y")
                {
                    Console.Write("Children (sequence of ids, any separator): ");
                    children = ConsoleExtensions.ReadIntList(Console.ReadLine());
                }

                (PersonProfile?, List<string>) result = await _personProfileService.UpdatePersonProfileAsync(personProfile, parents, children);

                Console.WriteLine();

                if (result.Item1 == null)
                {
                    Console.WriteLine("Could not update");
                }
                else
                {
                    Console.WriteLine($"Person with id {result.Item1.Id} has been successfully updated");
                }

                foreach (string message in result.Item2)
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
