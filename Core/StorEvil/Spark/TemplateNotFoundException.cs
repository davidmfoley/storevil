using System;

namespace StorEvil.Spark
{
    public class TemplateNotFoundException : Exception
    {
        public string Path { get; set; }

        public TemplateNotFoundException(string path)
        {
            Path = path;
        }

        public override string Message
        {
            get { return string.Format("Could not find the spark template at: '{0}'", Path); }
        }
    }
}