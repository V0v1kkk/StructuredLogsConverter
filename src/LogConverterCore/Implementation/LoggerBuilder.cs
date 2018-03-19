using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogConverterCore.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace LogConverterCore.Implementation
{
    /// <summary>
    /// Immutable!
    /// </summary>
    public class LoggerBuilder : ILoggerBuilder
    {
        private readonly string _outputTemplate;
        private readonly bool _splitByComponent;
        private readonly IEnumerable<string> _componentsList;
        private readonly LogEventLevel _minimumLevel;

        public LoggerBuilder(string outputTemplate)
        {
            _outputTemplate = outputTemplate;
        }

        private LoggerBuilder(string outputTemplate, bool splitByComponent, IEnumerable<string> componentsList,
            LogEventLevel minimumLevel)
        {
            _outputTemplate = outputTemplate;
            _splitByComponent = splitByComponent;
            _componentsList = componentsList;
            _minimumLevel = minimumLevel;
        }

        public ILoggerBuilder AddSplittingByComponents(IEnumerable<string> componentsList)
        {
            return new LoggerBuilder(_outputTemplate, true, componentsList, _minimumLevel);
        }

        /// <summary>
        /// Verbose by default.
        /// </summary>
        /// <param name="minimumLevel"></param>
        /// <returns></returns>
        public ILoggerBuilder SetMinimumLevel(LogEventLevel minimumLevel)
        {
            return new LoggerBuilder(_outputTemplate, _splitByComponent, _componentsList, minimumLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="outputFolder">If null - outputFolder = originFolder.</param>
        /// <returns></returns>
        public ILogger Build(string fileName, string outputFolder = null)
        {
            if (_splitByComponent && (_componentsList?.Count() ?? 0) < 1)
                throw new Exception("Components list is null or empty");

            var folder = Path.GetDirectoryName(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            if (!string.IsNullOrEmpty(outputFolder))
            {
                folder = outputFolder;
            }

            var outputPathAndFileName = Path.Combine(folder, fileNameWithoutExtension);

            var baseLoggerConfig = new LoggerConfiguration()
                .WriteTo.File(
                    new MessageTemplateTextFormatter(_outputTemplate, null),
                    path: $"{outputPathAndFileName}.txt",
                    restrictedToMinimumLevel: _minimumLevel,
                    fileSizeLimitBytes: long.MaxValue,
                    levelSwitch: null,
                    buffered: false,
                    shared: false,
                    flushToDiskInterval: null,
                    rollingInterval: RollingInterval.Infinite,
                    rollOnFileSizeLimit: false,
                    retainedFileCountLimit: null,
                    encoding: null);

            if (!_splitByComponent) return baseLoggerConfig.CreateLogger();

            foreach (var component in _componentsList)
            {
                baseLoggerConfig = baseLoggerConfig.WriteTo.Logger(
                    logger => logger.Filter.ByIncludingOnly(
                            logEvent =>
                            {
                                if (logEvent.Properties.TryGetValue("Component", out var componentName))
                                {
                                    return componentName.ToString() == component;
                                }

                                return false;
                            })
                        .WriteTo.File(
                            new MessageTemplateTextFormatter(_outputTemplate, null),
                            path: $"{outputPathAndFileName}_{component}.txt",
                            restrictedToMinimumLevel: _minimumLevel,
                            fileSizeLimitBytes: long.MaxValue,
                            levelSwitch: null,
                            buffered: false,
                            shared: false,
                            flushToDiskInterval: null,
                            rollingInterval: RollingInterval.Infinite,
                            rollOnFileSizeLimit: false,
                            retainedFileCountLimit: null,
                            encoding: null));
            }

            return baseLoggerConfig.CreateLogger();
        }
    }
}