using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Configuration
{
    public class ConfigParser : IConfigParser
    {
        public ConfigSettings Read(string contents)
        {       
            var nonBlankLines = GetNonBlankLines(contents);
            var lines = StripCommentLines(nonBlankLines);

            var settings = new ConfigSettings();

            foreach (var line in lines)
            {
                var indexOfSeparator = line.IndexOf(":");
               
                string settingName;
                string settingValue;

                if (indexOfSeparator > 0)
                {
                    settingName = line.Substring(0, indexOfSeparator).Trim().ToLowerInvariant();
                    settingValue = line.Substring(indexOfSeparator + 1).Trim();
                }
                else
                {
                    settingName = line;
                    settingValue = "";
                }       
                
                if (!ApplySettingValue(settings, settingName, settingValue))
                {
                    throw new BadSettingNameException( settingName, line);
                }
            }
            return settings;
        }

        private IEnumerable<string> StripCommentLines(IEnumerable<string> nonBlankLines)
        {
            return nonBlankLines.Where(l => !(l.Trim().StartsWith("#")));
        }

        private static bool ApplySettingValue(ConfigSettings settings, string settingName, string settingValue)
        {
            if (settingName == "assemblies")
                settings.AssemblyLocations = SplitSettingValue(settingValue);
            else if (settingName == "extensions")
                settings.ScenarioExtensions = SplitSettingValue(settingValue);
            else if (settingName == "outputfile")
                settings.OutputFile = settingValue;
            else if (settingName == "outputfileformat")
                settings.OutputFileFormat = settingValue;
            else if (settingName == "outputfiletemplate")
                settings.OutputFileTemplate = settingValue;
            else
                return false;

            return true;
        }

        private static IEnumerable<string> SplitSettingValue(string value)
        {
            return value.Split(',').Select(x => x.Trim());
        }

        private static IEnumerable<string> GetNonBlankLines(string contents)
        {
            return (contents ?? "")
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .Where(x => !string.IsNullOrEmpty(x));
        }
    }
}