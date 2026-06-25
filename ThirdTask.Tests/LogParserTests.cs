using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;
using ThirdTask;
using ThirdTask.Configuration;
using ThirdTask.Parsing;
using ThirdTask.Processing;

namespace ThirdTask.Tests
{
    public class LogProcessorTests : IDisposable
    {
        private readonly string _inputPath;
        private readonly string _outputPath;
        private readonly string _problemPath;

        public LogProcessorTests()
        {
            _inputPath = Path.GetTempFileName();
            _outputPath = Path.GetTempFileName();
            _problemPath = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(_inputPath)) File.Delete(_inputPath);
            if (File.Exists(_outputPath)) File.Delete(_outputPath);
            if (File.Exists(_problemPath)) File.Delete(_problemPath);
        }

        private LogProcessor CreateProcessor()
        {
            var parser = new CompositeLogParser(new ILogParser[]
            {
            new RegexLogParser(new()
            {
                Name = "Format1",
                Pattern = @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<loglevel>INFORMATION|WARNING|ERROR|DEBUG)\s+(?<message>.*)$",
                DefaultInvoker = "DEFAULT"
            }),
            new RegexLogParser(new()
            {
                Name = "Format2",
                Pattern = @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{4})\|\s+(?<loglevel>INFO|WARN|ERROR|DEBUG)\|(\d+)\|(?<invoker>[^|]*)\|\s+(?<message>.*)$",
                DefaultInvoker = "DEFAULT"
            })
            });
            return new LogProcessor(parser, new LogFormatter(), NullLogger<LogProcessor>.Instance);
        }

        [Fact]
        public async Task ProcessAsync_ValidLines_WrittenToOutput()
        {
            await File.WriteAllTextAsync(_inputPath,
                "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'\n" +
                "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства\n");

            var processor = CreateProcessor();
            await processor.ProcessAsync(_inputPath, _outputPath, _problemPath);

            var output = await File.ReadAllTextAsync(_outputPath);
            var problems = await File.ReadAllTextAsync(_problemPath);

            Assert.Contains("10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'", output);
            Assert.Contains("10-03-2025\t15:14:51.5882\tINFO\tMobileComputer.GetDeviceId\tКод устройства", output);
            Assert.Empty(problems.Trim());
        }

        [Fact]
        public async Task ProcessAsync_InvalidLines_WrittenToProblems()
        {
            await File.WriteAllTextAsync(_inputPath,
                "10.03.2025 15:14:49.523 INFORMATION Валидная строка\n" +
                "это не лог\n" +
                "ещё одна невалидная\n");

            var processor = CreateProcessor();
            await processor.ProcessAsync(_inputPath, _outputPath, _problemPath);

            var problems = await File.ReadAllTextAsync(_problemPath);

            Assert.Contains("это не лог", problems);
            Assert.Contains("ещё одна невалидная", problems);
        }

        [Fact]
        public async Task ProcessAsync_MissingInputFile_Throws()
        {
            var processor = CreateProcessor();
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                processor.ProcessAsync("nonexistent.log", _outputPath, _problemPath));
        }
    }
}
