using System;
using System.IO;

namespace WinFormsApp2.src.Services
{
    public static class LogsManager
    {
        private static readonly string logFilePath;

        static LogsManager()
        {
            // Proje dizininde Logs klasörü oluştur
            string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string logsFolder = Path.Combine(basePath, "Logs");

            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            // Kendine özel zaman damgalı dosya
            logFilePath = Path.Combine(logsFolder, $"game_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            File.AppendAllText(logFilePath, "=== OYUN BAŞLADI ===\n");
        }

        // Her olay için log kaydı
        public static void Log(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
    }
}
