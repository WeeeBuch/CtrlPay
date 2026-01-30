using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend;

public static class AppLogger
{
    private record LogEntry(string Level, string Message, string? Detail = null);

    private static readonly Channel<LogEntry> _logChannel;

    private static readonly string _logPath;
    private static readonly Task _processingTask;

    static AppLogger()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Roaming
        string folder = Path.Combine(appData, "CtrlPay");

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        _logPath = Path.Combine(folder, "app_logs.txt");

        _logChannel = Channel.CreateUnbounded<LogEntry>();

        // Spustíme konzumenta na pozadí (Fire-and-forget task)
        // LongRunning hint říká .NETu, že tohle vlákno poběží furt
        _processingTask = Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
    }

    private static void Log(string message, string level, string? detail = null)
    {
        _logChannel.Writer.TryWrite(new LogEntry(level, message, detail));
    }

    public static void Error(string message, Exception? ex = null) => Log(message, "ERROR", ex?.ToString());
    public static void Info(string message) => Log(message, "INFO");
    public static void Warning(string message) => Log(message, "WARNING");

    private static async Task ProcessLogQueue()
    {
        try
        {
            // Čte zprávy, dokud nějaké jsou. Když nejsou, await čeká (nezatěžuje CPU)
            await foreach (var entry in _logChannel.Reader.ReadAllAsync())
            {
                try
                {
                    string logLine = FormatLogMessage(entry);

                    // AppendAllText otevírá a zavírá soubor. Pro masivní logování by byl lepší Stream,
                    // ale pro běžné použití je to OK a bezpečnější (nezamyká soubor trvale).
                    await File.AppendAllTextAsync(_logPath, logLine, Encoding.UTF8);
                }
                catch
                {
                    // Tady nemáme kam logovat chybu logování, takže to ignorujeme,
                    // aby to nezhroutilo background thread.
                }
            }
        }
        catch (Exception)
        {
            // Kritická chyba loggeru
        }
    }

    public static void FlushAndClose()
    {
        try
        {
            // Řekneme kanálu, že už nic dalšího nepřijde
            _logChannel.Writer.Complete();

            // Počkáme, než se fronta vyprázdní a dopíše na disk.
            // Dáme tomu timeout třeba 2 sekundy, aby aplikace nezamrzla, kdyby se něco seklo.
            _processingTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch
        {
            // Pokud se to nepovede, nedá se nic dělat, ale aplikace se aspoň vypne.
        }
    }

    private static string FormatLogMessage(LogEntry entry)
    {
        var sb = new StringBuilder();
        sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
        sb.Append($" [{entry.Level}] ");
        sb.Append(entry.Message);

        if (!string.IsNullOrWhiteSpace(entry.Detail))
        {
            sb.AppendLine();
            sb.Append($"\tDetail: {entry.Detail}");
        }
        sb.AppendLine();
        return sb.ToString();
    }

    public static string GetLogPath() => _logPath;
}
