using System;

namespace StorEvil.Configuration
{
    public class BadSettingNameException : Exception
    {
        public BadSettingNameException(string settingName, string location)
        {
            SettingName = settingName;
            Location = location;
        }

        public override string Message
        {
            get
            {
                return string.Format("Error parsing '{0}'. There is no StorEvil setting named '{1}'", Location, SettingName);
            }
        }

        public string SettingName { get; private set; }
        public string Location { get; set; }
    }
}