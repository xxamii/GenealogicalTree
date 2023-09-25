using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.ViewModels;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree.Commands
{
    public class PersonProfilePageCommand : IPageCommand
    {
        private readonly IPersonProfileService _personProfileService;

        public PersonProfilePageCommand(IPersonProfileService personProfileService)
        {
            _personProfileService = personProfileService;
        }

        public async Task ExecuteAsync()
        {
            Console.Write("Person's id: ");
            int id = ConsoleExtensions.ReadInt(Console.ReadLine());

            PersonProfile? personProfile = (await _personProfileService.GetPersonProfilesAsync(a => a.Id == id)).FirstOrDefault();
            
            Console.WriteLine();

            if (personProfile != null)
            {
                ShowPersonData(personProfile);
                await ShowPersonRelatives(personProfile);
            }
            else
            {
                Console.WriteLine($"There is no person with id {id}");
            }
        }

        public void ShowPage()
        {
            throw new NotImplementedException();
        }

        private void ShowPersonData(PersonProfile personProfile)
        {
            StringBuilder result = new StringBuilder();
            result.Append($"{personProfile.Name}\n");
            result.Append($"{personProfile.Birthday.Day}.{personProfile.Birthday.Month}.{personProfile.Birthday.Year}\n");
            result.Append($"{personProfile.Country}\n");
            result.Append($"{personProfile.City}");

            Console.WriteLine(result);
        }

        private async Task ShowPersonRelatives(PersonProfile personProfile)
        {
            Console.Write("Show relatives in tree view? (y or n): ");
            string answer = ConsoleExtensions.ReadSpecificString(Console.ReadLine().ToLower(), new List<string> { "y", "n" });

            Console.WriteLine();

            if (answer.Contains("y"))
            {
                PersonRelatives personRelatives = await _personProfileService.GetPersonRelativesAsync(personProfile);

                if (personRelatives != null)
                {
                    string result = CreateRelativesTree(personRelatives);
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Something went wrong. Could not show relatives");
                }
            }
        }

        // │ └ ─ ├ ┌
        private string CreateRelativesTree(PersonRelatives person)
        {
            StringBuilder result = new StringBuilder();

            result.Append(CreateParentsTree(person, "\t"));
            result.Append($"({person.PersonProfile.Id}){person.PersonProfile.Name}\n");
            result.Append(CreateChildrenTree(person, "\t"));

            return result.ToString();
        }

        private string CreateParentsTree(PersonRelatives person, string offset = "")
        {
            StringBuilder result = new StringBuilder();

            foreach (PersonRelatives parent in person.Parents)
            {
                result.Insert(0, $"{offset}│\n");
                result.Insert(0, $"{offset}├── ({parent.PersonProfile.Id}){parent.PersonProfile.Name}\n");
                result.Insert(0, $"{CreateParentsTree(parent, offset+ "│\t")}");
            }

            return result.ToString();
        }

        private string CreateChildrenTree(PersonRelatives person, string offset = "")
        {
            StringBuilder result = new StringBuilder();

            foreach (PersonRelatives child in person.Children)
            {
                result.Append($"{offset}│\n");
                result.Append($"{offset}├── ({child.PersonProfile.Id}){child.PersonProfile.Name}\n");
                result.Append($"{CreateChildrenTree(child, offset+ "│\t")}");
            }

            return result.ToString();
        }
    }
}
