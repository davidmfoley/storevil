using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace StorEvil
{
    public class AppConfigFileReader : IConfigFileReader
    {
        public ConfigSettings Read(string filePath)
        {
            var appSettingsSections = new List<AppSettingsSection>
                                          {
                                              ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings
                                          };

            var map = new ExeConfigurationFileMap
                          {
                              LocalUserConfigFilename = filePath
                          };

            if (filePath != null)
                appSettingsSections.Add(ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.PerUserRoamingAndLocal).AppSettings);
     
            return BuildConfigSettings(appSettingsSections);
        }

        private ConfigSettings BuildConfigSettings(List<AppSettingsSection> sections)
        {
            return new ConfigSettings
                       {
                           AssemblyLocations = GetAssemblyLocations(sections),
                       };
        }

        public IEnumerable<string> GetAssemblyLocations(List<AppSettingsSection> sections)
        {
            var settings = GetAllSettings(sections, "contextAssembly");
            foreach (var setting in settings)
            {
                foreach (var s in setting.Split(','))
                    yield return s;
            }
        }


        private IEnumerable<string> GetAllSettings(List<AppSettingsSection> sections, string key)
        {
            foreach (var section in sections)
            {
                if (section.Settings[key].Key == key)
                    yield return section.Settings[key].Value;
            }
        }
    }

}