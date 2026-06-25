namespace ThirdTask.Processing;

using Microsoft.Extensions.Logging;
using ThirdTask.Parsing;

/// <summary>
/// Обрабатывает лог-файл: читает, парсит, форматирует, записывает.
/// Использует батчинг для производительности и поддерживает CancellationToken.
/// </summary>
public sealed class LogProcessor : ILogProcessor
{
    private const int BatchSize = 1000;

    private readonly ILogParser _parser;
    private readonly LogFormatter _formatter;
    private readonly ILogger<LogProcessor> _logger;

    public LogProcessor(
        ILogParser parser,
        LogFormatter formatter,
        ILogger<LogProcessor> logger)
    {
        _parser = parser;
        _formatter = formatter;
        _logger = logger;
    }

    public async Task ProcessAsync(
        string inputPath,
        string outputPath,
        string problemPath,
        CancellationToken cancellationToken = default)
    {
        ValidatePaths(inputPath, outputPath, problemPath);

        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath, append: false);
        using var problemWriter = new StreamWriter(problemPath, append: false);

        var validBatch = new List<string>(BatchSize);
        var invalidBatch = new List<string>(BatchSize);

        int lineNumber = 0;
        int totalValid = 0;
        int totalInvalid = 0;

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lineNumber++;

            var result = _parser.TryParse(line);

            if (result.IsSuccess && result.Entry is not null)
            {
                validBatch.Add(_formatter.Format(result.Entry));
                totalValid++;

                if (validBatch.Count >= BatchSize)
                {
                    await WriteBatchAsync(writer, validBatch, cancellationToken);
                    validBatch.Clear();
                }
            }
            else
            {
                invalidBatch.Add(line);
                totalInvalid++;

                if (invalidBatch.Count >= BatchSize)
                {
                    await WriteBatchAsync(problemWriter, invalidBatch, cancellationToken);
                    invalidBatch.Clear();
                }

                _logger.LogDebug(
                    "Invalid log at line {LineNumber}: {ErrorMessage}",
                    lineNumber,
                    result.ErrorMessage);
            }
        }

        // Записываем остатки
        if (validBatch.Count > 0)
            await WriteBatchAsync(writer, validBatch, cancellationToken);
        if (invalidBatch.Count > 0)
            await WriteBatchAsync(problemWriter, invalidBatch, cancellationToken);

        _logger.LogInformation(
            "Processing complete. Total: {Total}, Valid: {Valid}, Invalid: {Invalid}",
            lineNumber,
            totalValid,
            totalInvalid);
    }

    private static async Task WriteBatchAsync(
        StreamWriter writer,
        List<string> batch,
        CancellationToken cancellationToken)
    {
        foreach (var line in batch)
        {
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
        await writer.FlushAsync(cancellationToken);
    }

    private static void ValidatePaths(string inputPath, string outputPath, string problemPath)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"Input file not found: {inputPath}");

        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

        if (string.IsNullOrWhiteSpace(problemPath))
            throw new ArgumentException("Problem path cannot be empty", nameof(problemPath));
    }
}