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
    internal class MainPageCommand : IPageCommand
    {
        private readonly ISession _session;

        public MainPageCommand(ISession session)
        {
            _session = session;
        }

        public async Task ExecuteAsync()
        {
            ShowPage();
        }

        public void ShowPage()
        {
            StringBuilder page = new StringBuilder();
            page.Append("WELCOME TO THE GENEALOGICAL TREE\nCHOOSE A PAGE TO GO TO AND TYPE DOWN THE ACCORDING COMMAND\n\n");

            if (_session.IsUserLoggedIn && _session.CurrentUser != null)
            {
                page.Append($"Welcome, {_session.CurrentUser.Username}\n");
                page.Append("Logout (logout)\n");

                if (_session.CurrentUser.Role == Role.Administrator)
                {
                    page.Append("Add a person to the database (create)\n");
                    page.Append("Delete a person from the database (delete)\n");
                    page.Append("Update a person in the database (update)\n");
                }
            }
            else
            {
                page.Append("Register (register)\n");
                page.Append("Login (login)\n");
            }

            page.Append("Main (main)\n");
            page.Append("Search (search)\n");
            page.Append("Open person profile (person)\n");
            page.Append("Exit (exit)");

            Console.WriteLine(page);
        }
    }
}
