using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.Nunit
{
    /// <summary>
    /// Generates NUnit fixtures
    /// </summary>
    public class NUnitFixtureGenerator : IFixtureGenerator
    {
        private readonly IScenarioPreprocessor _preprocessor;

        public NUnitFixtureGenerator(IScenarioPreprocessor preprocessor, NUnitTestMethodGenerator methodGenerator)
        {
            _preprocessor = preprocessor;
            MethodGenerator = methodGenerator;
        }

        public NUnitTestMethodGenerator MethodGenerator { get; set; }

        private const string FixtureFormat =
            @"

namespace {0} {{
    {1}
    

    [TestFixture]
    {5}
    public class {2} {{
#line 1 ""{6}""
#line hidden
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {{
            {4}
        }}
        
{3}
    }}
}}";

        private static string GetFixtureName(Story story)
        {
            var name = story.Id;
            if (name.Contains(":\\"))
                name = name.After(":\\");

            if (name.Contains("."))
                name = name.Substring(0, name.LastIndexOf("."));

            return name.Replace("\\", "_").Replace(" ", "_") + "_Specs";

        }

        public string GenerateFixture(Story story, StoryContext context)
        {
            var tests = new StringBuilder();

            var contextSet = new TestContextSet();
            IEnumerable<string> namespaces = new string[0];

            foreach (var scenario in story.Scenarios)
            {
                foreach (var s in _preprocessor.Preprocess(scenario))
                {
                    NUnitTest test = MethodGenerator.GetTestFromScenario(s, context);
                    tests.Append("        " + test.Body);
                    tests.AppendLine();
                    contextSet.AddRange(test.ContextTypes);
                    namespaces = namespaces.Union(test.Namespaces).Distinct();
                }
            }

            
            var usingStatements = namespaces.Select(x => string.Format("using {0};", x));
            var usings = string.Join("\r\n", usingStatements.ToArray());

            var writeStoryToConsole = "Console.WriteLine(@\"" + story.Summary.Replace("\"", "\"\"") + "\r\n" + " \");";
            const string ns = "TestNamespace";
            var categories = string.Join("", (story.Tags ?? new string[0]).Select(t => string.Format(@"[Category(""{0}"")]", t)).ToArray());
            return string.Format(FixtureFormat, ns, usings, GetFixtureName(story), tests, writeStoryToConsole, categories, story.Id);
        }
    }
}