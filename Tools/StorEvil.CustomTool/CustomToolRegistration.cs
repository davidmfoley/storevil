using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace StorEvil.CustomTool
{
    public class CustomToolRegistration
    {
        private const string path =
            @"SOFTWARE\Microsoft\VisualStudio\visual_studio_version\Generators\{fae04ec1-301f-11d3-bf4b-00c04f79efbc}\";

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {

            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(path + "StorEvilCustomTool"))
            {
                key.SetValue("", "Generates tests from StorEvil spec files");
                key.SetValue("CLSID", "{" + StorEvilCustomTool.ClsId + "}");
                key.SetValue("GeneratesDesignTimeSource", 1);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            Registry.LocalMachine.DeleteSubKey("StorEvilCustomTool");              
        }
    }

    class CustomToolRegistrationImpl : CustomToolRegistration
    {
    }
}