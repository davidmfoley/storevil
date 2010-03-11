using System;
using System.IO;

namespace StorEvil.Infrastructure
{
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
            System.Console.WriteLine("writing file: " + OutputFile);
            using (var stream = File.AppendText(OutputFile))
            {
                stream.Write(s);
            }
        }
    }
}