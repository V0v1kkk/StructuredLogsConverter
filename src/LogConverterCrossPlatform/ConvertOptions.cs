using System.Collections.Generic;
using CommandLine;
using Serilog.Events;

namespace LogConverterCrossPlatform
{
    //"Convert compact JSON formatted structured log to old txt format."
    public class ConvertOptions
    {
        [Option('s', "source", Required = true,
            HelpText = "Source file path. Allow multiple files/folders.")] //todo: files support!!!
        public IEnumerable<string> Sources { get; set; }

        [Option("onelevel", Required = false, Default = false,
            HelpText = "Search only in target folder(s), not in subfolders.")]
        public bool OneLevelSerach { get; set; }

        [Option("template",
            Default = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {ProcessId} [{ThreadId}] {SourceContext}: {Message}{NewLine}{Exception}",
            Required = false,
            HelpText = "Output log-file format.")]
        public string OutputTemplate { get; set; }

        [Option('t', "target", Required = false,
            HelpText = "Output files folder. By default the same as origin folder.")]
        public string TargetPath { get; set; }

        [Option('c', "component", Default = false, Required = false,
            HelpText = "Split output log-files by component.")] //todo: change default
        public bool SplitByComponent { get; set; }

        ////todo: component key

        [Option("componentlist", Required = false,
            HelpText = "Componet list. If not specified, it will be extracted from the files.")]
        public IEnumerable<string> ComponentList { get; set; }

        [Option('l', "level", Required = false, Default = LogEventLevel.Verbose,
            HelpText = "Minimum log level.")]
        public LogEventLevel LogEventLevel { get; set; }

        //[HelpText(true, true, true, "Vladimir Rogozhin 2018", "LogConverter", 200)]
        //public string DoHelpForVerb(string verbName)
        //{
        //    var help = HelpText.AutoBuild<ConvertOptions>(new ParserResult<ConvertOptions>(ParserResultType.Parsed, typeof(ConvertOptions)));
        //    help.Copyright = ;

        //    return help;
        //}
    }
}