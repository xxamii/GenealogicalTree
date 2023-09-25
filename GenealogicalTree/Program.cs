using GenealogicalTree;

PageCommandInvoker pageCommandInvoker = DependencyFactory.GetPageCommandInvoker();

string command = "main";

while (command != ConsoleInterfaceCommands.ExitCommand)
{
    await pageCommandInvoker.ExecuteCommand(command);

    Console.WriteLine();
    Console.Write("Where to?: ");
    command = Console.ReadLine();
    Console.WriteLine();
}
