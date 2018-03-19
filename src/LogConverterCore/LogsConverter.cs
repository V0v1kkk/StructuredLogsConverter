using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LogConverterCore.Interfaces;
using Serilog;
using Serilog.Events;

namespace LogConverterCore
{
    public class LogsConverter
    {
        private readonly ILogger _systemLogger;
        private readonly ILogFinder _logFinder;
        private readonly IComponentsExtractor _componentsExtractor;
        private readonly ILogFileConverterFactory _converterFactory;
        private readonly ILoggerBuilder _loggerBuilder;


        public LogsConverter(ILogger systemLogger, ILogFinder logFinder, IComponentsExtractor componentsExtractor,
                             ILoggerBuilder loggerBuilder, ILogFileConverterFactory converterFactory)
        {
            _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
            _logFinder = logFinder ?? throw new ArgumentNullException(nameof(logFinder));
            _componentsExtractor = componentsExtractor ?? throw new ArgumentNullException(nameof(componentsExtractor));
            _loggerBuilder = loggerBuilder ?? throw new ArgumentNullException(nameof(loggerBuilder));
            _converterFactory = converterFactory ?? throw new ArgumentNullException(nameof(converterFactory));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="outputFolder"></param>
        /// <param name="splitByComponents">If splitByComponents is true, log file be slitted to separate files by component property.</param>
        /// <param name="componetsList">If null or empty - component list be extracted form logfile.</param>
        /// <param name="minimumLevel"></param>
        /// <param name="oneLevelSearch"></param>
        public void ConvertLogs(string folderPath, string outputFolder,
                                bool splitByComponents,
                                IEnumerable<string> componetsList = null, 
                                LogEventLevel minimumLevel = LogEventLevel.Verbose,
                                bool oneLevelSearch = false)
        {
            // local function
            void ConvertLogFile(string logFile, ILoggerBuilder loggerBuilder, bool deleteOrigin = false)
            {
                var fileComponentsList = componetsList;

                if (splitByComponents)
                {
                    if (!fileComponentsList.IsAny())
                    {
                        fileComponentsList = _componentsExtractor.ExtractComponentsList(logFile).GetAwaiter().GetResult();
                    }

                    loggerBuilder = _loggerBuilder.AddSplittingByComponents(fileComponentsList);
                }

                var logger = loggerBuilder.Build(logFile, outputFolder);
                var converter = _converterFactory.CreateConverter(logger);
                converter.ConvertFile(logFile, deleteOrigin);
            }


            if (outputFolder == null) outputFolder = folderPath;

            try
            {
                var logFiles = _logFinder.FindLogFilesInFolder(folderPath, oneLevelSearch);
                var logFilesUnpacked = _logFinder.FindAndUnpackLogArchivesInFolder(folderPath, oneLevelSearch);

                var loggerBuilder = _loggerBuilder.SetMinimumLevel(minimumLevel);
                
                foreach (var logFile in logFiles)
                {
                    try
                    {
                        _systemLogger.Verbose("Converting log file: {FileName}", logFile);

                        ConvertLogFile(logFile, loggerBuilder);
                    }
                    catch (Exception exception)
                    {
                        _systemLogger.Error(exception, "Error on converting file: {FileName}", logFile);
                    }
                }

                foreach (var logFile in logFilesUnpacked)
                {
                    try
                    {
                        _systemLogger.Verbose("Converting unpacked log file: {FileName}", logFile);

                        ConvertLogFile(logFile, loggerBuilder, true); //delete unpacked file after convertation
                    }
                    catch (Exception exception)
                    {
                        _systemLogger.Error(exception, "Error on converting file from archive: {FileName}", logFile);
                    }
                }
            }
            catch (Exception exception)
            {
                _systemLogger.Error(exception, "Error on converting log files.");
                throw;
            }
        }


    }
}