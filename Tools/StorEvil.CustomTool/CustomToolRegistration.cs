using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace StorEvil.CustomTool
{
    public abstract class CustomToolRegistration
    {
        private const string PrefixPath32Bit = @"SOFTWARE\";
        private const string PrefixPath64Bit = @"SOFTWARE\Wow6432Node\";
        private const string InnerPath = @"Microsoft\VisualStudio\10.0\Generators\{fae04ec1-301f-11d3-bf4b-00c04f79efbc}\";
        private const string StorEvilKey = "StorEvilCustomTool";

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            using (var key = Registry.LocalMachine.CreateSubKey(GetPath(), RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                System.Console.WriteLine("Registering at: " + key.Name);
                key.SetValue("", "Generates tests from StorEvil spec files");
                key.SetValue("CLSID", "{" + StorEvilCustomTool.ClsId + "}");
                key.SetValue("GeneratesDesignTimeSource", 1);
                key.Flush();
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            System.Console.WriteLine("Trying to remove StorEvilCustomTool from: {0}", GetPath());
            Registry.LocalMachine.DeleteSubKeyTree(GetPath());
        }

        private static string GetPath()
        {
            var prefix = Is64BitOS() ? PrefixPath64Bit : PrefixPath32Bit;

            return prefix + InnerPath + StorEvilKey;          
        }

        /// <summary>
        /// TODO: this is a hack, move to Environment.Is64BitOs once code migrated to .NET 4
        /// Copied from: http://stackoverflow.com/questions/336633/how-to-detect-windows-64-bit-platform-with-net
        /// </summary>
        /// <returns></returns>
        private static bool Is64BitOS()
        {
            return (IntPtr.Size == 8) || InternalCheckIsWow64();
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool InternalCheckIsWow64()
        {
            var osVersion = Environment.OSVersion.Version;

            if ((osVersion.Major != 5 || osVersion.Minor < 1) && osVersion.Major < 6)
                return false;

            using (Process p = Process.GetCurrentProcess())
            {
                bool retVal;
                return IsWow64Process(p.Handle, out retVal) && retVal;
            }
        }
    }
}