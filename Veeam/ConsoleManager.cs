using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Veeam
{
    public static class ConsoleManager
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Repository"/> class with user-specified configuration.
        /// </summary>
        /// <remarks>This method prompts the user to provide paths for the source directory, replica
        /// directory, and log directory, as well as a time interval for periodic checks. If an error occurs during the
        /// setup process, the method logs the error message to the console and returns <see
        /// langword="null"/>.</remarks>
        /// <returns>A new <see cref="Repository"/> instance configured with the specified directories and check interval, or
        /// <see langword="null"/> if an error occurs during the setup process.</returns>
        public static Repository? CreateRepository()
        {
            try
            {
                string sourceDirectory = SetupDirectory("Enter the source path: ");
                string replicaDirectory = SetupDirectory("Enter the copy path: ");
                string logDirectory = SetupDirectory("Enter the log file path: ");

                TimeSpan checkInterval = GetTimeSpan("Enter the check interval (e.g., 00:05:00 for 5 minutes): ");

                return new Repository(sourceDirectory, replicaDirectory, logDirectory, checkInterval);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Prompts the user to specify a directory path, validates it, and ensures the directory exists.
        /// </summary>
        /// <remarks>If the specified directory does not exist, the user is given the option to create it.
        /// If the user declines, the operation is cancelled, and an empty string is returned.</remarks>
        /// <param name="message">The message to display when prompting the user for a directory path.</param>
        /// <returns>The absolute path of the directory specified by the user.  Returns an empty string if the operation is
        /// cancelled by the user.</returns>
        private static string SetupDirectory(string message)
        {
            string directory = GetUserInput(message);
            directory.IsValidAbsoluteDirectoryPath();

            if (!Directory.Exists(directory))
            {
                if (GetUserConfirmation($"Directory '{directory}' does not exist. Would you like to create it?"))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Directory '{directory}' created successfully.");
                }
                else
                {
                    Console.WriteLine("Operation cancelled. Exiting.");
                    return string.Empty;
                }
            }
            return directory;
        }

        /// <summary>
        /// Prompts the user with a specified message and retrieves their input.
        /// </summary>
        /// <param name="message">The message to display to the user as a prompt.</param>
        /// <returns>The input entered by the user as a string. If the user provides no input, an empty string is returned.</returns>
        private static string GetUserInput(string message)
        {
            Console.Write(message);
            string? intpur = Console.ReadLine();
            return intpur ?? string.Empty;
        }
        /// <summary>
        /// Prompts the user with a message and retrieves a confirmation response.
        /// </summary>
        /// <param name="message">The message to display to the user, asking for confirmation.</param>
        /// <returns><see langword="true"/> if the user responds with "y" (case-insensitive); otherwise, <see langword="false"/>.</returns>

        private static bool GetUserConfirmation(string message)
        {
            Console.Write(message + " (y/n): ");
            string? response = Console.ReadLine();
            return response?.Trim().ToLower() == "y";
        }
        /// <summary>
        /// Determines whether the specified string is a valid absolute directory path.
        /// </summary>
        /// <remarks>On Windows, the path must be rooted to a drive letter (e.g., "C:\"), and
        /// platform-specific path validation is applied.</remarks>
        /// <param name="path">The path to validate.</param>
        /// <returns><see langword="true"/> if the specified path is a valid absolute directory path; otherwise, <see
        /// langword="false"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is <see langword="null"/>, empty, contains invalid characters, is not an
        /// absolute path, or does not conform to platform-specific requirements (e.g., drive letter rooting on
        /// Windows).</exception>
        private static bool IsValidAbsoluteDirectoryPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.");
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException("Path contains invalid characters.");

            if (!Path.IsPathRooted(path))
                throw new ArgumentException("Path must be absolute.");

            if (OperatingSystem.IsWindows())
            {
                var root = Path.GetPathRoot(path);
                if (string.IsNullOrEmpty(root) || !Regex.IsMatch(root, @"^[A-Za-z]:\\$"))
                    throw new ArgumentException("Path must be rooted to a drive letter (e.g., C:\\).");
            }

            return true;
        }
        /// <summary>
        /// Determines whether the specified <see cref="Repository"/> instance was created successfully.
        /// </summary>
        /// <remarks>This method logs a message to the console indicating whether the repository was
        /// created successfully or not.</remarks>
        /// <param name="repository">The <see cref="Repository"/> instance to validate. Can be <see langword="null"/>.</param>
        /// <returns>The same <see cref="Repository"/> instance if it is not <see langword="null"/>; otherwise, <see
        /// langword="null"/>.</returns>
        public static Repository? IsCreatedCorrectly(this Repository? repository)
        {
            if (repository is null)
            {
                Console.WriteLine("Repository creation failed. Please check the inputs and try again.");
            }
            else
            {
                Console.WriteLine("Repository created successfully.");
                Console.WriteLine(repository);
            }
            return repository;
        }
        /// <summary>
        /// Prompts the user for a <see cref="TimeSpan"/> input and parses it.
        /// </summary>
        /// <remarks>If the user provides an invalid <see cref="TimeSpan"/> format, a message is
        /// displayed, and the default value of 5 minutes is returned.</remarks>
        /// <param name="message">The message to display to the user when prompting for input.</param>
        /// <returns>The parsed <see cref="TimeSpan"/> value if the input is valid; otherwise, a default value of 5 minutes.</returns>
        private static TimeSpan GetTimeSpan(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (TimeSpan.TryParse(input, out TimeSpan result))
            {
                return result;
            }
            else
            {
                Console.WriteLine("Invalid TimeSpan format. Using default value of 5 minutes.");
                return TimeSpan.FromMinutes(5);
            }
        }
    }
}
