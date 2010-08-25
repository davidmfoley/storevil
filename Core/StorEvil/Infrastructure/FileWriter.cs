using System.IO;
using StorEvil.Interpreter;

namespace StorEvil.Infrastructure
{
    public class FileWriter : ITextWriter
    {
        private readonly bool _overwrite;
        public string OutputFile { get; private set; }

        public FileWriter(string outputFile, bool overwrite)
        {
            _overwrite = overwrite;
            OutputFile = outputFile;
        }

        public void Write(string s)
        {
            if (_overwrite && File.Exists(OutputFile))
                File.Delete(OutputFile);

            DebugTrace.Trace("FileWriter", "writing file: " + OutputFile);
            using (var stream = File.AppendText(OutputFile))
            {
                stream.Write(s);
            }
        }
    }
}