using System;

namespace StorEvil.Configuration
{
    public class BadSettingNameException : Exception
    {
        public BadSettingNameException(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; private set; }
    }
}