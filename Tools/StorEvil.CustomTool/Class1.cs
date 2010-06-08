using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using StorEvil.Configuration;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.CustomTool
{
    [Guid(ClsId)]
    public class StorEvilCustomTool : IVsSingleFileGenerator 
    {
        public const string ClsId = "EAC0AD56-60E8-4528-A286-87D810F09C55";

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            throw new NotImplementedException();
        }
        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint bytesWritten, IVsGeneratorProgress progress)
        {
            //var generator = new CustomToolCodeGenerator();
            //var code = generator.GenerateCode(inputFilePath);

            //var bytes = Encoding.UTF8.GetBytes(code);
            //var length = bytes.Length;
           
            bytesWritten = (uint) 0; //length;

            //outputFileContents[0] = Marshal.AllocCoTaskMem(length);           
            //Marshal.Copy(bytes, 0, outputFileContents[0], length);
            return VSConstants.S_OK;
        }  
    }

   

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
}
