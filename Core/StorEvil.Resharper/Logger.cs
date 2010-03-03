using System;
using System.IO;

namespace StorEvil.Resharper
{
    internal class Logger
    {
        private static DateTime _lastLogTime = DateTime.MinValue;

        public static void Log(string msg)
        {
            using (var stream = File.AppendText("C:\\storevil.log"))
            {
                if (DateTime.Now > _lastLogTime.AddSeconds(2))
                {
                    stream.WriteLine("\r\n-------------------------------------");
                    stream.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
                    stream.WriteLine("-------------------------------------\r\n");
                }

                stream.WriteLine(msg);

                Console.WriteLine(msg);

                _lastLogTime = DateTime.Now;
            }
        }
    }
}