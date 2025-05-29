// See https://aka.ms/new-console-template for more information

using Veeam;


ConsoleInput consoleInput = new ConsoleInput();

Repository repository = consoleInput.CreateRepository();

Synchronizer synchronizer = new Synchronizer(repository);

await synchronizer.SynchronizeFolders();

