using System.Collections.Generic;
using Serilog;
using Serilog.Events;

namespace LogConverterCore.Interfaces
{
    public interface ILoggerBuilder
    {
        ILoggerBuilder AddSplittingByComponents(IEnumerable<string> componentsList);
        ILoggerBuilder SetMinimumLevel(LogEventLevel minimumLevel);
        ILogger Build(string filePath, string outputFolder);
    }
}