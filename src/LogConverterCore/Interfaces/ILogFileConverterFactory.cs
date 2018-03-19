using Serilog;

namespace LogConverterCore.Interfaces
{
    public interface ILogFileConverterFactory
    {
        IFileConverter CreateConverter(ILogger logger);
    }
}