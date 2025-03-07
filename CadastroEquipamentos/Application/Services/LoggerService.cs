using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Application.Services
{
    public class LoggerService
    {
        private string LogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Logs/EventLogs");
        private string ErrorLogDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Logs/ErrorLogs");

        public LoggerService()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            if (!Directory.Exists(ErrorLogDirectory))
            {
                Directory.CreateDirectory(ErrorLogDirectory);
            }
        }

        private string GetLogFilePath(bool isErrorLog = false)
        {
            string programName = "EquipmentManagement";

            string directory = isErrorLog ? ErrorLogDirectory : LogDirectory;
            string fileName = $"{programName}_log_{DateTime.Now:yyyyMMdd}.txt";

            return Path.Combine(directory, fileName);
        }

        public async Task LogAsync(string userAction, string logMessage)
        {
            string logEntry = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {userAction} - {logMessage}\n";

            string filePath = GetLogFilePath();

            await File.AppendAllTextAsync(filePath, logEntry);
        }

        public async Task LogErrorAsync(string userAction, string errorMessage)
        {
            string logEntry = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - ERROR - {userAction} - {errorMessage}\n";

            string filePath = GetLogFilePath(isErrorLog: true);

            await File.AppendAllTextAsync(filePath, logEntry);
        }
    }
}
