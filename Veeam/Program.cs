// See https://aka.ms/new-console-template for more information

using Veeam;

Repository repository = ConsoleInput.CreateRepository();
if (repository == null)
{
    Console.WriteLine("Failed to create repository. Exiting.");
    return;
}
else
{
    SynchronizationChecker checker = new SynchronizationChecker(repository);
    await checker.StartChecking();
}


