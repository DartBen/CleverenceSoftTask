namespace ThirdTask.Processing;

public interface ILogProcessor
{
    Task ProcessAsync(
        string inputPath,
        string outputPath,
        string problemPath,
        CancellationToken cancellationToken = default);
}