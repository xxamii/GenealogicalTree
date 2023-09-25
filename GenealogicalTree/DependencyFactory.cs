using BLL.Abstractions.Interfaces;
using GenealogicalTree.Commands;
using PL.Abstractions.Interfaces;

namespace GenealogicalTree
{
    internal class DependencyFactory
    {
        private static ISession _session;
        private static PageCommandInvoker _invoker;

        public static PageCommandInvoker GetPageCommandInvoker()
        {
            if (_invoker == null)
            {
                Dictionary<string, IPageCommand> commands = new Dictionary<string, IPageCommand>();
                ISession session = BLL.DependencyFactory.GetSession();
                IPersonProfileService personProfileService = BLL.DependencyFactory.GetPersonProfileService();

                MainPageCommand mainPageCommand = new MainPageCommand(session);
                LoginPageCommand loginPageCommand = new LoginPageCommand(session);
                LogoutPageCommand logoutPageCommand = new LogoutPageCommand(session);
                RegisterPageCommand registerPageCommand = new RegisterPageCommand(session);
                SearchPageCommand searchPageCommand = new SearchPageCommand(personProfileService);
                PersonProfilePageCommand personProfilePage = new PersonProfilePageCommand(personProfileService);
                CreatePersonProfilePageCommand createPersonProfilePageCommand = new CreatePersonProfilePageCommand(session, personProfileService);
                DeletePersonProfilePageCommand deletePersonProfilePageCommand = new DeletePersonProfilePageCommand(session, personProfileService);
                UpdatePersonProfilePageCommand updatePersonProfilePageCommand = new UpdatePersonProfilePageCommand(session, personProfileService);

                commands.Add(ConsoleInterfaceCommands.MainPageCommand, mainPageCommand);
                commands.Add(ConsoleInterfaceCommands.LoginPageCommand, loginPageCommand);
                commands.Add(ConsoleInterfaceCommands.LogoutPageCommand, logoutPageCommand);
                commands.Add(ConsoleInterfaceCommands.RegisterPageCommand, registerPageCommand);
                commands.Add(ConsoleInterfaceCommands.SearchPageCommand, searchPageCommand);
                commands.Add(ConsoleInterfaceCommands.PersonPageCommand, personProfilePage);
                commands.Add(ConsoleInterfaceCommands.CreatePersonProfileCommand, createPersonProfilePageCommand);
                commands.Add(ConsoleInterfaceCommands.DeletePersonProfileCommand, deletePersonProfilePageCommand);
                commands.Add(ConsoleInterfaceCommands.UpdatePersonProfileCommand, updatePersonProfilePageCommand);

                _invoker = new PageCommandInvoker(commands);
            }

            return _invoker;
        }
    }
}
