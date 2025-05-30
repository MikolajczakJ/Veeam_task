using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veeam
{
    public class Logger
    {
        private readonly string _logFileName;
        private string _logFilePath;
        public Logger(Repository repository)
        {
            _logFilePath = repository.LogDirectory;
            _logFileName = new DirectoryInfo(repository.SourceDirectory).Name;
            _logFilePath = Path.Combine(_logFilePath, $"{_logFileName}_logs.log");
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Dispose();
            }
        }
        /// <summary>
        /// Asynchronously writes a log message to the log file and outputs it to the console.
        /// </summary>
        /// <remarks>The log entry includes a timestamp in the format of the current system date and time.
        /// If an error occurs while writing to the log file, the exception message is written to the console.</remarks>
        /// <param name="message">The message to log. Cannot be null or empty.</param>
        /// <returns></returns>
        public async Task Log(string message)
        {
            string log = string.Empty;
            try
            {
                using (var writer = new StreamWriter(_logFilePath, true))
                {
                    log = $"{DateTime.Now}: {message}";
                    await writer.WriteLineAsync(log);
                    Console.WriteLine(log);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
