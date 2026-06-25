namespace ThirdTask.Configuration;

/// <summary>
/// Конфигурация форматов логов. Загружается из appsettings.json.
/// Позволяет добавлять новые форматы без изменения кода (OCP).
/// </summary>
public sealed class LogFormatsConfiguration
{
    public List<LogFormatDescriptor> Formats { get; set; } = new();
}

public sealed class LogFormatDescriptor
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
    public string DefaultInvoker { get; set; } = "DEFAULT";
}