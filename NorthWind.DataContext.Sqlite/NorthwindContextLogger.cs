using Microsoft.Extensions.Logging;
using static System.Environment;

namespace Northwind.EntityModels;

public class NorthwindContextLogger
{
    public static void WriteLine(string message, LogLevel logLevel = LogLevel.Information)
    {
        if (logLevel == LogLevel.Error) {

        string path = Path.Combine(Environment.CurrentDirectory, "northwindlog.txt");

        StreamWriter textFile = File.AppendText(path);
        textFile.WriteLine(message);
        textFile.Close();
        }
    }

    public static void WriteLine(string message, ICollection<LogLevel> logLevels)
    {
        if (logLevels.Contains(LogLevel.Error) || logLevels.Contains(LogLevel.Warning))
        {
            string path = Path.Combine(Environment.CurrentDirectory, "northwindlog.txt");

            StreamWriter textFile = File.AppendText(path);
            textFile.WriteLine(message);
            textFile.Close();
        }
    }
}