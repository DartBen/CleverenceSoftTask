namespace ThirdTask.Parsing;

using System.Text.RegularExpressions;
using ThirdTask.Configuration;
using ThirdTask.Models;

/// <summary>
/// Парсер на основе регулярного выражения.
/// Использует именованные группы для извлечения полей.
/// </summary>
public sealed class RegexLogParser : ILogParser
{
    private static readonly HashSet<string> RequiredGroups = new()
    {
        "date", "time", "loglevel", "message"
    };

    private readonly Regex _regex;
    private readonly string _defaultInvoker;

    public string Name { get; }

    public RegexLogParser(LogFormatDescriptor descriptor)
    {
        Name = descriptor.Name;
        _defaultInvoker = descriptor.DefaultInvoker;
        _regex = new Regex(descriptor.Pattern, RegexOptions.Compiled);
        ValidatePattern();
    }

    private void ValidatePattern()
    {
        var actualGroups = _regex.GetGroupNames().ToHashSet();
        foreach (var required in RequiredGroups)
        {
            if (!actualGroups.Contains(required))
                throw new ArgumentException(
                    $"Regex for format '{Name}' is missing required group: {required}");
        }
    }

    public ParseResult TryParse(string line)
    {
        var match = _regex.Match(line);
        if (!match.Success)
            return ParseResult.Failure($"Line does not match format '{Name}'");

        if (!DateOnly.TryParse(match.Groups["date"].Value, out var date))
            return ParseResult.Failure("Invalid date format");

        var invoker = match.Groups["invoker"].Success
            ? match.Groups["invoker"].Value.Trim()
            : _defaultInvoker;

        if (string.IsNullOrWhiteSpace(invoker))
            invoker = _defaultInvoker;

        var entry = new LogEntry(
            Date: date,
            Time: match.Groups["time"].Value,
            LogLevel: NormalizeLogLevel(match.Groups["loglevel"].Value),
            InvokerMethod: invoker,
            Message: match.Groups["message"].Value);

        return ParseResult.Success(entry);
    }

    private static string NormalizeLogLevel(string level) => level switch
    {
        "INFORMATION" => "INFO",
        "WARNING" => "WARN",
        "INFO" or "WARN" or "ERROR" or "DEBUG" => level,
        _ => "UNDEFINED"
    };
}