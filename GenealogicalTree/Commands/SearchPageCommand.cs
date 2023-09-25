using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    public class SearchPageCommand : IPageCommand
    {
        private readonly IPersonProfileService _personProfileService;

        public SearchPageCommand(IPersonProfileService personProfileService)
        {
            _personProfileService = personProfileService;
        }

        public async Task ExecuteAsync()
        {
            ShowPage();

            Console.Write("Name (or leave empty): ");
            string name = Console.ReadLine();

            Console.Write("Country (or leave empty): ");
            string country = Console.ReadLine();

            Console.Write("City (or leave empty): ");
            string city = Console.ReadLine();

            Console.Write("Birth year (or leave empty): ");
            string yearInput = Console.ReadLine();
            int year = 0;

            while(yearInput.Trim().Length > 0 && !int.TryParse(yearInput, out year))
            {
                Console.Write("Enter a valid number or leave empty: ");
                yearInput = Console.ReadLine();
            }

            if (name.Trim().Length < 1 &&
                country.Trim().Length < 1 &&
                city.Trim().Length < 1 &&
                yearInput.Trim().Length < 1)
            {
                Console.WriteLine("Sorry, at least some filters have to be specified");
                return;
            }

            Console.WriteLine();

            Func<PersonProfile, bool> predicate = (person) => {
                bool nameMatched = name.Trim().Length < 1 ? true : person.Name.ToLower().Contains(name.Trim().ToLower());
                bool countryMatched = country.Trim().Length < 1 ? true : person.Country.ToLower().Contains(country.Trim().ToLower());
                bool cityMatched = city.Trim().Length < 1 ? true :  person.City.ToLower().Contains(city.Trim().ToLower());
                bool yearMatched = yearInput.Trim().Length < 1 ? true : year == person.Birthday.Year;

                return nameMatched && countryMatched && cityMatched && yearMatched;
            };

            List<PersonProfile> personProfiles = await _personProfileService.GetPersonProfilesAsync(predicate);

            if (personProfiles.Count() > 0)
            {
                foreach (PersonProfile personProfile in personProfiles)
                {
                    Console.WriteLine($"({personProfile.Id}) {personProfile.Name} - {personProfile.Country} - {personProfile.Birthday.Day}.{personProfile.Birthday.Month}.{personProfile.Birthday.Year}");
                }
            }
            else
            {
                Console.WriteLine("Sorry, could not find people under your search filters");
            }
        }

        public void ShowPage()
        {
            Console.WriteLine("Please enter someone's information\n");
        }
    }
}
