using SharpShell.ServerRegistration;
using System;
using Autofac;
using LogConverter.Operations;
using LogConverterCore;
using Serilog;

namespace LogConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string invokedVerb = null;
            object invokedVerbInstance = new object();

            var options = new AllOperations();

            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
                {
                    // if parsing succeeds the verb name and correct instance
                    // will be passed to onVerbCommand delegate (string,object)
                    invokedVerb = verb;
                    invokedVerbInstance = subOptions;
                }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            var menu = new ContextMenuExtension();
            switch (invokedVerb)
            {
                case "install":
                    ServerRegistrationManager.InstallServer(menu, RegistrationType.OS64Bit, true);
                    ServerRegistrationManager.RegisterServer(menu, RegistrationType.OS64Bit);
                    break;
                case "unistall":
                    ServerRegistrationManager.UnregisterServer(menu, RegistrationType.OS64Bit);
                    ServerRegistrationManager.UninstallServer(menu, RegistrationType.OS64Bit);
                    break;
                case "convert":
                    var convertOptions = (ConvertOptions)invokedVerbInstance;
                    Convert(convertOptions);
                    break;
            }

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
                    Log.Fatal("Initialization error.", exception);


                    var defaultForeground = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"Error on folder convertation: {folder}");

                    Console.ForegroundColor = defaultForeground;
                }
            }

            Log.CloseAndFlush();

            Console.WriteLine("Done!");
        }
    }
}
