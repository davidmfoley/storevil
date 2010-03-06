using System;
using System.IO;

namespace StorEvil.ResultListeners
{
    public interface IFileWriter
    {
        void Write(string s);
    }

    public class FileWriter : IFileWriter
    {
        public string OutputFile { get; private set; }

        public FileWriter(string outputFile, bool overwrite)
        {
            OutputFile = outputFile;
            if (overwrite)
                File.Delete(OutputFile);

            Console.WriteLine(Path.GetFullPath(outputFile));
        }

        public void Write(string s)
        {
            using (var stream = File.AppendText(OutputFile))
            {
                stream.Write(s);
            }
        }
    }
}