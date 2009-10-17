using System;

namespace StorEvil.Nunit
{
    internal class ScenarioLineImplementation
    {
        private readonly string _code;

        public TestContextField Context { get; set; }

        public ScenarioLineImplementation(string code)
        {
            _code = code;
           
        }

        public ScenarioLineImplementation(string code, Type contextType, string contextFieldName)
        {
            _code = code;
            Context = new TestContextField {Name = contextFieldName, Type = contextType};
           
        }

        public string Code
        {
            get { return _code; }
        }

       
    }
}