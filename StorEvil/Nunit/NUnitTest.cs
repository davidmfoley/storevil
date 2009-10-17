using System.Collections.Generic;

namespace StorEvil.Nunit
{
    public class NUnitTest
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public IEnumerable<TestContextField> ContextTypes { get; private set; }

        public NUnitTest(string name, string body, IEnumerable<TestContextField> contextTypes)
        {
            Name = name;
            Body = body;
            ContextTypes = contextTypes;
        }
    }
}