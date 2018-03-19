using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogConverterCore.Interfaces;
using Serilog;
using Serilog.Formatting.Compact.Reader;

namespace LogConverterCore.Implementation
{
    public class ComponentsExtractor : IComponentsExtractor
    {
        private readonly ILogger _systemLogger;

        public ComponentsExtractor(ILogger systemLogger)
        {
            _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
        }

        public Task<IList<string>> ExtractComponentsList(string filePath)
        {
            return Task.Factory.StartNew(() =>
            {
                _systemLogger.ForContext("MethodName", nameof(ExtractComponentsList))
                    .Verbose("Method {MethodName} enter. File name: {FilenName}.", filePath);

                var componentsHashset = new HashSet<string>();

                try
                {
                    using (var logFileStream = File.OpenText(filePath))
                    {
                        var reader = new LogEventReader(logFileStream);
                        while (reader.TryRead(out var logEvent))
                        {
                            if (!logEvent.Properties.Keys.Contains("Component")) continue;
                            if (!componentsHashset.Contains(logEvent.Properties["Component"].ToString()))
                            {
                                componentsHashset.Add(logEvent.Properties["Component"].ToString());
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _systemLogger.ForContext("MethodName", nameof(ExtractComponentsList)).Error(exception, "Method error");
                }

                return componentsHashset.ToList() as IList<string>;
            });
        }
    }
}