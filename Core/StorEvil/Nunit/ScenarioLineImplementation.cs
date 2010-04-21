using System;
using System.Collections.Generic;

namespace StorEvil.NUnit
{
    internal class ScenarioLineImplementation
    {
        public IEnumerable<string> Namespaces { get; set; }
        private readonly string _code;

        public TestContextField Context { get; set; }

        public ScenarioLineImplementation(string code)
        {
            _code = code;
        }

        public ScenarioLineImplementation(string code, Type contextType, string contextFieldName, IEnumerable<string> namespaces)
        {
            Namespaces = namespaces;
            _code = code;
            Context = new TestContextField {Name = contextFieldName, Type = contextType};
        }

        public string Code
        {
            get { return _code; }
        }
    }
}