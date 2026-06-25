namespace ThirdTask.Parsing;

using ThirdTask.Models;

public interface ILogParser
{
    string Name { get; }
    ParseResult TryParse(string line);
}