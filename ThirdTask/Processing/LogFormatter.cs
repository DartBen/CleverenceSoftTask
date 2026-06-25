namespace ThirdTask.Processing;

using System.Text;
using ThirdTask.Models;

/// <summary>
/// Форматирует LogEntry в выходную строку.
/// Вынесено в отдельный класс для соблюдения SRP и упрощения тестирования.
/// </summary>
public sealed class LogFormatter
{
    private const char Separator = '\t';

    public string Format(LogEntry entry)
    {
        var sb = new StringBuilder();
        sb.Append(entry.Date.ToString("dd-MM-yyyy"));
        sb.Append(Separator);
        sb.Append(entry.Time);
        sb.Append(Separator);
        sb.Append(entry.LogLevel);
        sb.Append(Separator);
        sb.Append(entry.InvokerMethod);
        sb.Append(Separator);
        sb.Append(entry.Message);
        return sb.ToString();
    }
}