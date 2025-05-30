using System.Text;

namespace Veeam.Tests
{
    public class ConsoleManagerTests
    {
        [Fact]
        public void CreateRepository_ValidInput_ReturnsRepository()
        {
            // Arrange
            var input = new StringBuilder();
            input.AppendLine(@"C:\Source");
            input.AppendLine(@"C:\Replica");
            input.AppendLine(@"C:\Logs");
            input.AppendLine("00:10:00");
            using var inputReader = new StringReader(input.ToString());
            Console.SetIn(inputReader);

            var output = new StringWriter();
            Console.SetOut(output);

            Directory.CreateDirectory(@"C:\Source");
            Directory.CreateDirectory(@"C:\Replica");
            Directory.CreateDirectory(@"C:\Logs");

            // Act
            var repo = ConsoleManager.CreateRepository();

            // Assert
            Assert.NotNull(repo);
            Assert.Equal(@"C:\Source", repo.SourceDirectory);
            Assert.Equal(@"C:\Replica", repo.ReplicaDirectory);
            Assert.Equal(@"C:\Logs", repo.LogDirectory);
            Assert.Equal(TimeSpan.FromMinutes(10), repo.CheckInterval);

            // Cleanup
            Directory.Delete(@"C:\Source");
            Directory.Delete(@"C:\Replica");
            Directory.Delete(@"C:\Logs");
        }

        [Fact]
        public void CreateRepository_InvalidPath_ThrowsAndReturnsNull()
        {
            // Arrange
            var input = new StringBuilder();
            input.AppendLine("Invalid|Path");
            using var inputReader = new StringReader(input.ToString());
            Console.SetIn(inputReader);

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            var repo = ConsoleManager.CreateRepository();

            // Assert
            Assert.Null(repo);
            Assert.Contains("Path contains invalid characters", output.ToString());
        }

        [Fact]
        public void CreateRepository_DirectoryDoesNotExist_UserDeclinesCreation_ReturnsNull()
        {
            // Arrange
            var input = new StringBuilder();
            input.AppendLine(@"C:\NotExist1");
            input.AppendLine("n");
            using var inputReader = new StringReader(input.ToString());
            Console.SetIn(inputReader);

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            var repo = ConsoleManager.CreateRepository();

            // Assert
            Assert.Null(repo);
            Assert.Contains("Operation cancelled. Exiting.", output.ToString());
        }

        [Fact]
        public void CreateRepository_InvalidTimeSpan_UsesDefault()
        {
            // Arrange
            var input = new StringBuilder();
            input.AppendLine(@"C:\Source2");
            input.AppendLine(@"C:\Replica2");
            input.AppendLine(@"C:\Logs2");
            input.AppendLine("notatimespan");
            using var inputReader = new StringReader(input.ToString());
            Console.SetIn(inputReader);

            var output = new StringWriter();
            Console.SetOut(output);

            Directory.CreateDirectory(@"C:\Source2");
            Directory.CreateDirectory(@"C:\Replica2");
            Directory.CreateDirectory(@"C:\Logs2");

            // Act
            var repo = ConsoleManager.CreateRepository();

            // Assert
            Assert.NotNull(repo);
            Assert.Equal(TimeSpan.FromMinutes(5), repo.CheckInterval);
            Assert.Contains("Invalid TimeSpan format", output.ToString());

            // Cleanup
            Directory.Delete(@"C:\Source2");
            Directory.Delete(@"C:\Replica2");
            Directory.Delete(@"C:\Logs2");
        }
    }
}
