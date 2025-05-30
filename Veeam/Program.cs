// See https://aka.ms/new-console-template for more information

using Veeam;

Repository? repository = null; ;
//Repository creation loop until a repository with valid directories is created
do
{
    repository= ConsoleManager.CreateRepository().IsCreatedCorrectly();
} while (repository is null);

SynchronizationChecker checker = new SynchronizationChecker(repository!);
await checker.StartChecking();


