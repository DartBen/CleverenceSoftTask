namespace ThirdTask.Models;

/// <summary>
/// Result-паттерн для обработки ошибок парсинга без исключений.
/// Позволяет явно работать с успешными и неуспешными результатами.
/// </summary>
public readonly struct ParseResult
{
    public bool IsSuccess { get; }
    public LogEntry? Entry { get; }
    public string? ErrorMessage { get; }

    private ParseResult(LogEntry entry)
    {
        IsSuccess = true;
        Entry = entry;
        ErrorMessage = null;
    }

    private ParseResult(string errorMessage)
    {
        IsSuccess = false;
        Entry = null;
        ErrorMessage = errorMessage;
    }

    public static ParseResult Success(LogEntry entry) => new(entry);
    public static ParseResult Failure(string errorMessage) => new(errorMessage);

    public void Deconstruct(out bool isSuccess, out LogEntry? entry, out string? errorMessage)
    {
        isSuccess = IsSuccess;
        entry = Entry;
        errorMessage = ErrorMessage;
    }
}