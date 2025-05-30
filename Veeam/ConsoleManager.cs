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
        private static readonly string _directoryMessage = "Enter the directory path: ";
        private static readonly string _replicaMessage = "Enter the directory path: ";
        private static readonly string _logMessage = "Enter the log file path: ";
        private static readonly string _checkIntervalMessage = "Enter the check interval (e.g., 00:05:00 for 5 minutes): ";
        public static Repository? CreateRepository()
        {
            try
            {
                string? sourceDirectory = SetupDirectory(_directoryMessage);
                string? replicaDirectory = SetupDirectory(_replicaMessage);
                string? logDirectory = SetupDirectory(_logMessage);

                TimeSpan checkInterval = GetTimeSpan(_checkIntervalMessage);

                return new Repository(sourceDirectory, replicaDirectory, logDirectory, checkInterval);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private static string? SetupDirectory(string message)
        {
            string directory = GetDirectory(message);
            if (!directory.IsValidAbsoluteDirectoryPath())
            {
                return null;
            }
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
        private static string GetDirectory(string message)
        {
            Console.Write(message);
            string? directory = Console.ReadLine();
            return directory ?? string.Empty;
        }

        private static bool GetUserConfirmation(string message)
        {
            Console.Write(message + " (y/n): ");
            string? response = Console.ReadLine();
            return response?.Trim().ToLower() == "y";
        }
        private static bool IsValidAbsoluteDirectoryPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.");
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException("Path contains invalid characters.");

            try
            {
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
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static Repository? IsCreatedCorrectly(this Repository? repository)
        {
            if (repository is null)
            {
                Console.WriteLine("Repository creation failed. Please check the inputs and try again.");
            }
            else
            {
                Console.WriteLine("Repository created successfully.");
                Console.WriteLine($"Source Directory: {repository.SourceDirectory}");
                Console.WriteLine($"Replica Directory: {repository.ReplicaDirectory}");
                Console.WriteLine($"Log Directory: {repository.LogDirectory}");
                Console.WriteLine($"Check Interval: {repository.CheckInterval}");
            }
            return repository;
        }
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
