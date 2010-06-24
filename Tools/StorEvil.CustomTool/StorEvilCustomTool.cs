using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using StorEvil.CodeGeneration;
using StorEvil.Configuration;
using StorEvil.Infrastructure;
using StorEvil.Parsing;

namespace StorEvil.CustomTool
{
    [Guid(ClsId)]
    public class StorEvilCustomTool : CustomToolRegistration, IVsSingleFileGenerator 
    {
        public const string ClsId = "EAC0AD56-60E8-4528-A286-87D810F09C55";

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".feature.cs";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint bytesWritten, IVsGeneratorProgress progress)
        {
            var generator = new FixtureGenerator();
            var code = generator.GenerateCode(inputFilePath, inputFileContents, defaultNamespace);

            var bytes = Encoding.UTF8.GetBytes(code);
            var length = bytes.Length;
           
            bytesWritten = (uint) length;

            outputFileContents[0] = Marshal.AllocCoTaskMem(length);           
            Marshal.Copy(bytes, 0, outputFileContents[0], length);
            return VSConstants.S_OK;
        }  
    }
}
