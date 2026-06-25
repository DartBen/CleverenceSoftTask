namespace ThirdTask.Configuration;

internal static class EmbeddedDefaults
{
    // JSON встроен прямо в код — не нужен внешний файл
    public const string DefaultFormatsJson = """
    {
      "LogFormats": {
        "Formats": [
          {
            "Name": "Format1",
            "Pattern": "^(?<date>\\d{2}\\.\\d{2}\\.\\d{4})\\s+(?<time>\\d{2}:\\d{2}:\\d{2}\\.\\d{3})\\s+(?<loglevel>INFORMATION|WARNING|ERROR|DEBUG)(?<invoker>)\\s+(?<message>.*)$",
            "DefaultInvoker": "DEFAULT"
          },
          {
            "Name": "Format2",
            "Pattern": "^(?<date>\\d{4}-\\d{2}-\\d{2})\\s+(?<time>\\d{2}:\\d{2}:\\d{2}\\.\\d{4})\\|\\s+(?<loglevel>INFO|WARN|ERROR|DEBUG)\\|(\\d+)\\|(?<invoker>[^|]*)\\|\\s+(?<message>.*)$",
            "DefaultInvoker": "DEFAULT"
          }
        ]
      }
    }
    """;
}