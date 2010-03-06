using StorEvil.Core.Configuration;

namespace StorEvil.Console
{
    internal class CommonSwitchParser : SwitchParser<ConfigSettings>
    {
        public CommonSwitchParser()
        {
            AddSwitch("--story-path", "-p")
                .SetsField(s => s.StoryBasePath)
                .WithDescription(
                    "Sets the base path used when searching for story files.\r\nIf not set, the current working directory is assumed.");

            AddSwitch("--assemblies", "-a")
                .SetsField(s => s.AssemblyLocations)
                .WithDescription(
                    "Sets the location (relative to current path) of the context assemblies used to parse the stories.");

            AddSwitch("--output-file", "-o")
                .SetsField(s => s.OutputFile)
                .WithDescription("If set, storevil will output to the specified file.");

            AddSwitch("--output-file-format", "-f")
                .SetsField(s => s.OutputFileFormat)
                .WithDescription(
                    "Sets the format of output to the file specified by --output-file (ONLY xml is supported so far)\r\n" +
                    "If nothing is specified, the output file location will be: storevil.output.{format}");

            AddSwitch("--console-mode", "-c")
                .SetsEnumField(s => s.ConsoleMode)  
                .WithDescription("Sets the format of output to the console (color, nocolor, quiet)");
        }
    }
}