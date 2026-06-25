using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using ThirdTask.Configuration;
using ThirdTask.Parsing;
using ThirdTask.Processing;

var rootCommand = new RootCommand("Консольная программа для стандартизации лог-файлов");

var inputOption = new Option<FileInfo>("--input", "-i")
{
    Description = "Путь к входному лог-файлу",
    DefaultValueFactory = _ => new FileInfo("input.log")
};

var outputOption = new Option<FileInfo>("--output", "-o")
{
    Description = "Путь к выходному файлу",
    DefaultValueFactory = _ => new FileInfo("output.log")
};

var problemOption = new Option<FileInfo>("--problems", "-p")
{
    Description = "Путь к файлу с невалидными строками",
    DefaultValueFactory = _ => new FileInfo("problems.txt")
};

rootCommand.Options.Add(inputOption);
rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(problemOption);

rootCommand.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
{
    // GetValue возвращает nullable — нужны проверки
    var input = parseResult.GetValue(inputOption)
        ?? new FileInfo("input.log");
    var output = parseResult.GetValue(outputOption)
        ?? new FileInfo("output.log");
    var problems = parseResult.GetValue(problemOption)
        ?? new FileInfo("problems.txt");

    var hostBuilder = Host.CreateDefaultBuilder(Array.Empty<string>())
        .ConfigureAppConfiguration((context, config) =>
        {
            // Подключаем конфигурацию форматов из appsettings.json
            // 1. Сначала — встроенные дефолты (самый низкий приоритет)
            config.AddJsonStream(
                new MemoryStream(System.Text.Encoding.UTF8.GetBytes(EmbeddedDefaults.DefaultFormatsJson)));

            // 2. Затем — файл appsettings.json, если есть (перекрывает дефолты)
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        })
        .ConfigureLogging(logging =>
        {
            logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
            logging.SetMinimumLevel(LogLevel.Information);
        })
        .ConfigureServices((context, services) =>
        {
            // 1. Конфигурация форматов
            services.Configure<LogFormatsConfiguration>(
                context.Configuration.GetSection("LogFormats"));

            // 2. Отдельные RegexLogParser для каждого формата
            services.AddSingleton<List<ILogParser>>(sp =>
            {
                var config = sp
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<LogFormatsConfiguration>>()
                    .Value;

                return config.Formats
                    .Select(f => (ILogParser)new RegexLogParser(f))
                    .ToList();
            });

            // 3. Композитный парсер — агрегирует все форматы
            services.AddSingleton<ILogParser>(sp =>
            {
                var parsers = sp.GetRequiredService<List<ILogParser>>();
                return new CompositeLogParser(parsers);
            });

            // 4. Форматтер выходных данных
            services.AddSingleton<LogFormatter>();

            // 5. Процессор логов
            services.AddSingleton<ILogProcessor, LogProcessor>();
        });

    using var host = hostBuilder.Build();
    await host.StartAsync(cancellationToken);

    var processor = host.Services.GetRequiredService<ILogProcessor>();
    await processor.ProcessAsync(
        input.FullName,
        output.FullName,
        problems.FullName,
        cancellationToken);

    await host.StopAsync(cancellationToken);
    return 0;
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();