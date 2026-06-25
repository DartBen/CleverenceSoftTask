namespace ThirdTask.Models;

/// <summary>
/// Успешно распарсенная запись лога.
/// Immutable record для потокобезопасности и простоты тестирования.
/// </summary>
public sealed record LogEntry(
    DateOnly Date,
    string Time,
    string LogLevel,
    string InvokerMethod,
    string Message);
