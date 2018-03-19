using System;
using System.IO;
using Autofac;
using AutofacSerilogIntegration;
using LogConverterCore;
using LogConverterCore.Implementation;
using LogConverterCore.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace LogConverter
{
    public static class AutofaConfigurationHelper
    {
        public static ContainerBuilder GetConfig(string outputTemplate = null)
        {
            outputTemplate = string.IsNullOrEmpty(outputTemplate)
                ? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {ProcessId} [{ThreadId}] {SourceContext}: {Message}{NewLine}{Exception}"
                : outputTemplate;

            var builder = new ContainerBuilder();

            InitializeLogging();
            builder.RegisterLogger();

            builder.RegisterType<LogsConverter>().AsSelf();

            builder.RegisterType<LogsFinderService>().As<ILogFinder>();

            builder.RegisterType<LoggerBuilder>().As<ILoggerBuilder>().
                WithParameter((parameterInfo, context) => parameterInfo.Name == "outputTemplate", 
                              (parameterInfo, context) => outputTemplate);

            builder.RegisterType<LogFileConverterFactory>().As<ILogFileConverterFactory>();

            builder.RegisterType<ComponentsExtractor>().As<IComponentsExtractor>();

            return builder;
        }

        private static void InitializeLogging()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose()
                .WriteTo.File(new CompactJsonFormatter(),
                              Path.Combine(Path.Combine(Environment.CurrentDirectory, "Log"), "log.json"),
                              LogEventLevel.Verbose,
                              buffered: false, shared: true)
                .CreateLogger();
        }
    }
}