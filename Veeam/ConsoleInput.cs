using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public class ConsoleInput
    {
        public Repository CreateRepository()
        {
            string sourceDirectory = SetupDirectory("Enter the source directory path: ");
            
            string replicaDirectory = SetupDirectory("Enter the replica directory path: ");
            
            string logDirectory = SetupDirectory("Enter the log file path: ");

            TimeSpan checkInterval = GetTimeSpan("Enter the check interval (e.g., 00:05:00 for 5 minutes): ");

            return new Repository(sourceDirectory, replicaDirectory, logDirectory, checkInterval);
        }
        private string SetupDirectory(string message)
        {
            string directory = GetDirectory(message);
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
        private string GetDirectory(string message)
        {
            Console.Write(message);
            string? directory = Console.ReadLine();
            return directory ?? string.Empty;
        }

        private bool GetUserConfirmation(string message)
        {
            Console.Write(message + " (y/n): ");
            string? response = Console.ReadLine();
            return response?.Trim().ToLower() == "y";
        }
        public static TimeSpan GetTimeSpan(string message)
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
