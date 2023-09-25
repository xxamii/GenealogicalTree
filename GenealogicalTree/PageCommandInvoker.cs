using PL.Abstractions.Interfaces;

namespace GenealogicalTree
{
    internal class PageCommandInvoker
    {
        private readonly Dictionary<string, IPageCommand> _commands;

        public PageCommandInvoker(Dictionary<string, IPageCommand> commands)
        {
            _commands = commands;
        }

        public async Task ExecuteCommand(string command)
        {
            if (_commands.ContainsKey(command.ToLower().Trim()))
            {
                await _commands[command.ToLower().Trim()].ExecuteAsync();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"There is no such page {command}");
                Console.WriteLine();
            }
        }
    }
}
