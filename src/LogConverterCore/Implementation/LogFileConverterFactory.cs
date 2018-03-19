using System;
using LogConverterCore.Interfaces;
using Serilog;

namespace LogConverterCore.Implementation
{
    public class LogFileConverterFactory : ILogFileConverterFactory
    {
        private readonly ILogger _systemLogger;

        public LogFileConverterFactory(ILogger systemLogger)
        {
            _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
        }

        public IFileConverter CreateConverter(ILogger logger)
        {
            return new LogFileConverter(logger, _systemLogger);
        }
    }
}