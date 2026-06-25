namespace ThirdTask.Parsing;

using ThirdTask.Models;

/// <summary>
/// Композитный парсер: пытается распарсить строку каждым парсером по очереди.
/// Возвращает первый успешный результат.
/// </summary>
public sealed class CompositeLogParser : ILogParser
{
    private readonly IReadOnlyList<ILogParser> _parsers;

    public string Name => "Composite";

    public CompositeLogParser(IEnumerable<ILogParser> parsers)
    {
        _parsers = parsers.ToList();
        if (_parsers.Count == 0)
            throw new ArgumentException("At least one parser is required");
    }

    public ParseResult TryParse(string line)
    {
        foreach (var parser in _parsers)
        {
            var result = parser.TryParse(line);
            if (result.IsSuccess)
                return result;
        }

        return ParseResult.Failure("Line does not match any known format");
    }
}