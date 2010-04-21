using System.Collections.Generic;

namespace StorEvil.NUnit
{
    public class NUnitTest
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public IEnumerable<TestContextField> ContextTypes { get; private set; }
        public IEnumerable<string> Namespaces { get; set; }

        public NUnitTest(string name, string body, IEnumerable<TestContextField> contextTypes, IEnumerable<string> namespaces)
        {
            Name = name;
            Body = body;
            ContextTypes = contextTypes;
            Namespaces = namespaces;
        }
    }
}