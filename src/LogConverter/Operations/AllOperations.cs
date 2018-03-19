using CommandLine;
using CommandLine.Text;

namespace LogConverter.Operations
{
    public class AllOperations
    {
        [VerbOption("convert", HelpText = "Convert compact JSON formatted structured log to old txt format.")]
        public ConvertOptions ConvertVerb { get; set; }

        [VerbOption("install", HelpText = "Install windows explorer extension, that provide log convertation by context menu.")]
        public InstallOptions InstallVerb { get; set; }

        [VerbOption("unistall", HelpText = "Install windows explorer extension.")]
        public UnistallOptions UnistallVerb { get; set; }

        [HelpVerbOption]
        public string DoHelpForVerb(string verbName)
        {
            var help = HelpText.AutoBuild(this, verbName);
            help.Copyright = "Vladimir Rogozhin 2018";

            return help;
        }
    }
}