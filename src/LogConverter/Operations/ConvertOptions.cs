using CommandLine;
using Serilog.Events;

namespace LogConverter.Operations
{
    public class ConvertOptions
    {
        [OptionArray('s', "source", Required = true,
            HelpText = "Source file path. Allow multiple files/folders.")] //todo: files support!!!
        public string[] Sources { get; set; }

        [Option("onelevel", Required = false, DefaultValue = false,
            HelpText = "Search only in target folder(s), not in subfolders.")]
        public bool OneLevelSerach { get; set; }

        [Option("template", 
            DefaultValue = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {ProcessId} [{ThreadId}] {SourceContext}: {Message}{NewLine}{Exception}", 
            Required = false,
            HelpText = "Output log-file format.")]
        public string OutputTemplate { get; set; }

        [Option('t', "target", Required = false,
            HelpText = "Output files folder. By default the same as origin folder.")]
        public string TargetPath { get; set; }

        [Option('c', "component", DefaultValue = false, Required = false,
            HelpText = "Split output log-files by component.")] //todo: change default
        public bool SplitByComponent { get; set; }

        //todo: component key

        [Option("componentlist", Required = false,
            HelpText = "Componet list. If not specified, it will be extracted from the files.")]
        public string[] ComponentList { get; set; }

        [Option('l', "level", Required = false, DefaultValue = Serilog.Events.LogEventLevel.Verbose,
            HelpText = "Minimum log level.")]
        public LogEventLevel LogEventLevel { get; set; }
    }
}