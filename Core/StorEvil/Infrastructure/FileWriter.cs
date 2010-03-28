using System;
using System.IO;
using StorEvil.Interpreter;

namespace StorEvil.Infrastructure
{
    public class FileWriter : ITextWriter
    {
        public string OutputFile { get; private set; }

        public FileWriter(string outputFile, bool overwrite)
        {
            OutputFile = outputFile;
            if (overwrite)
                File.Delete(OutputFile);

            
        }

        public void Write(string s)
        {
            DebugTrace.Trace("FileWriter", "writing file: " + OutputFile);
            using (var stream = File.AppendText(OutputFile))
            {
                stream.Write(s);
            }
        }
    }
}