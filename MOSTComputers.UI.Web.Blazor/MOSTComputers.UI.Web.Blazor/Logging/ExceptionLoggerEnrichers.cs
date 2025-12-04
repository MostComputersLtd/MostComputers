using Serilog.Core;
using Serilog.Events;

namespace MOSTComputers.UI.Web.Blazor.Logging;

public sealed class LoggerNameEnricher : ILogEventEnricher
{
    private readonly string _sourceContextPropertyName = "SourceContext";

    private readonly string _propertyName;

    public LoggerNameEnricher(string propertyName)
    {
        _propertyName = propertyName;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue(_sourceContextPropertyName, out LogEventPropertyValue? sourceContext))
        {
            LogEventProperty loggerName = propertyFactory.CreateProperty(_propertyName, sourceContext.ToString());

            logEvent.AddOrUpdateProperty(loggerName);
        }
    }
}

public class ExceptionTypeEnricher : ILogEventEnricher
{
    private readonly string _propertyName;

    public ExceptionTypeEnricher(string propertyName)
    {
        _propertyName = propertyName;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception != null)
        {
            string? exceptionType = logEvent.Exception.GetType().FullName;

            LogEventProperty property = propertyFactory.CreateProperty(_propertyName, exceptionType);

            logEvent.AddOrUpdateProperty(property);
        }
    }
}