using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil
{
    public interface IConfigFileReader
    {
        ConfigSettings Read(string filePath);
    }

    public class ConfigSettings
    {
        public IEnumerable<string> ScenarioExtensions { get; set; }
        public IEnumerable<string> AssemblyLocations { get; set; }

        public string StoryBasePath { get; set; }

        public static ConfigSettings Default()
        {
            return new ConfigSettings
                       {
                           ScenarioExtensions = new [] {".txt", ".feature", ".story"}, 
                           AssemblyLocations = new string[0]
                       };
        }
    }

    public class ConfigParser : IConfigFileReader
    {
        private readonly IFilesystem _filesystem;

        public ConfigParser(IFilesystem filesystem)
        {
            _filesystem = filesystem;
        }

        public ConfigSettings Read(string filePath)
        {
            var contents = _filesystem.GetFileText(filePath);

            var lines = GetNonBlankLines(contents);

            var settings = new ConfigSettings();

            foreach (var line in lines)
            {
                var indexOfSeparator = line.IndexOf(":");
               
                string settingName;
                string settingValue;

                if (indexOfSeparator > 0)
                {
                    settingName = line.Substring(0, indexOfSeparator).Trim().ToLowerInvariant();
                    settingValue = line.Substring(indexOfSeparator + 1);
                }
                else
                {
                    settingName = line;
                    settingValue = "";
                }
                
                ApplySettingValue(settings, settingName, settingValue);
            }
            return settings;
        }

        private static void ApplySettingValue(ConfigSettings settings, string settingName, string settingValue)
        {
            if (settingName == "assemblies")
            {
                settings.AssemblyLocations = SplitSettingValue(settingValue);
            }
            else if (settingName == "extensions")
            {
                settings.ScenarioExtensions = SplitSettingValue(settingValue);
            }
            else
            {
                throw new BadSettingNameException(settingName);
            }
        }

        private static IEnumerable<string> SplitSettingValue(string value)
        {
            return value.Split(',').Select(x => x.Trim());
        }

        private static IEnumerable<string> GetNonBlankLines(string contents)
        {
            return (contents ??"")
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .Where(x => !string.IsNullOrEmpty(x));
        }
    }

    public class BadSettingNameException : Exception
    {
        public BadSettingNameException(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; private set; }
    }
}