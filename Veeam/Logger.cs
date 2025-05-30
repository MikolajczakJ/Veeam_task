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
