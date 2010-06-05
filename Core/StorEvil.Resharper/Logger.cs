using System;
using System.Diagnostics;
using System.IO;

namespace StorEvil.Resharper
{
    internal class Logger
    {
        private static DateTime _lastLogTime = DateTime.MinValue;

        public static void Log(string msg)
        {
            System.Console.WriteLine(msg);
            Debug.WriteLine(msg);
           
            if (!EnableLogging)
                return;

            using (var stream = File.AppendText("C:\\projects\\storevil\\logs\\resharper.log"))
            {
                if (DateTime.Now > _lastLogTime.AddSeconds(2))
                {
                    stream.WriteLine("\r\n-------------------------------------");
                    stream.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
                    stream.WriteLine("-------------------------------------\r\n");
                }

                stream.WriteLine(msg);

                _lastLogTime = DateTime.Now;
            }
        }

        protected static bool EnableLogging = false;
    }
}