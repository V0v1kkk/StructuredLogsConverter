using System;
using System.IO;
using LogConverterCore.Interfaces;
using Serilog;
using Serilog.Formatting.Compact.Reader;

namespace LogConverterCore.Implementation
{
    public class LogFileConverter : IFileConverter
    {
        private readonly ILogger _targetLogger;
        private readonly ILogger _systemLogger;

        public LogFileConverter(ILogger targetLogger, ILogger systemLogger)
        {
            _targetLogger = targetLogger ?? throw new ArgumentNullException(nameof(targetLogger));
            _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
        }

        public void ConvertFile(string filePath, bool deleteOrigin)
        {
            _systemLogger.Verbose("Method {MethodName} enter. File name: {FilenName}. Delete file: {DeleteFileAfterRead}", 
                nameof(ConvertFile), filePath, deleteOrigin);

            try
            {
                using (var logFileStream = File.OpenText(filePath))
                {
                    var reader = new LogEventReader(logFileStream);
                    while (reader.TryRead(out var logEvent))
                    {
                        _targetLogger.Write(logEvent);
                    }
                }

                if (deleteOrigin)
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception exception)
            {
                _systemLogger.Error(exception, "Convertation error");
            }
        }

        
    }
}