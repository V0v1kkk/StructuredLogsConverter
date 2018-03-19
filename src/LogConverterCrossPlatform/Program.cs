using System;
using Autofac;
using CommandLine;
using LogConverterCore;
using Serilog;

namespace LogConverterCrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<ConvertOptions>(args)
                .WithParsed(Convert)
                .WithNotParsed(errors =>
                {
                    Environment.Exit(0);
                });
        }

        private static void Convert(ConvertOptions convertOptions)
        {
            var container = AutofaConfigurationHelper.GetConfig(convertOptions.OutputTemplate).Build();
            var logsConverter = container.Resolve<LogsConverter>();

            foreach (var folder in convertOptions.Sources)
            {
                try
                {
                    logsConverter.ConvertLogs(folderPath: folder.Trim('"'), // bug: commandline library don't trim '"'
                                              outputFolder: convertOptions.TargetPath,
                                              splitByComponents: convertOptions.SplitByComponent,
                                              componetsList: convertOptions.ComponentList,
                                              minimumLevel: convertOptions.LogEventLevel,
                                              oneLevelSearch: convertOptions.OneLevelSerach);
                }
                catch (Exception exception)
                {
                    Log.Fatal(exception, "Application fatal error.");

                    WriteErrorToConsole($"Error on folder convertation: {folder}");
                }
            }

            Log.CloseAndFlush();

            Console.WriteLine("Done!");
        }

        private static void WriteErrorToConsole(string text)
        {
            var defaultForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(text);

            Console.ForegroundColor = defaultForeground;
        }
    }
}
