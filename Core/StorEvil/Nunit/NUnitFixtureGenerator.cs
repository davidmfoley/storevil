using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.CodeGeneration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter.ParameterConverters;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
    /// <summary>
    /// Generates NUnit fixtures
    /// </summary>
    public class NUnitFixtureGenerator : IFixtureGenerator
    {
        private readonly IScenarioPreprocessor _preprocessor;

        public NUnitFixtureGenerator(IScenarioPreprocessor preprocessor, ITestMethodGenerator methodGenerator)
        {
            _preprocessor = preprocessor;
            MethodGenerator = methodGenerator;
        }

        public ITestMethodGenerator MethodGenerator { get; set; }


        private const string FixtureFormat =
            @"

namespace {0} {{
    {1}
    

    [TestFixture]
    {5}
    public class {2} : StorEvil.NUnit.StorEvilTestFixtureBase {{
#line 1 ""{6}""
#line hidden
        private StorEvil.Interpreter.ParameterConverters.ParameterConverter ParameterConverter = new StorEvil.Interpreter.ParameterConverters.ParameterConverter();
        [TestFixtureSetUp]
        public void FixtureSetUp() {{
            base.SetUp();
            {4}
        }}
        

        [TearDown]
        public void TearDown() {{
            base.CleanUpScenario();
        }}

        [TestFixtureTearDown]
        public void FixtureTearDown() {{
            base.CleanUpStory();
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
            return new TestFixtureClassGenerator().Generate(story, "StorEvilSpecs");
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
            const string ns = "StorEvilSpecifications";
            var categories = string.Join("", (story.Tags ?? new string[0]).Select(t => string.Format(@"[Category(""{0}"")]", t)).ToArray());
            return string.Format(FixtureFormat, ns, usings, GetFixtureName(story), tests, writeStoryToConsole, categories, story.Id);
        }

        public string GenerateSetupTearDown(ISessionContext sessionContext)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace StorEvilSpecs { [SetUpFixture] public class SetupAndTearDown {");
            sb.AppendLine("  [SetUp] public void SetUp() {");
            AppendAssemblySetup(sb, sessionContext);
            sb.AppendLine();
            sb.AppendLine("  }");
            sb.AppendLine("  [TearDown] public void TearDown() {");
            sb.AppendLine("  }");
            sb.AppendLine("} }");
            return sb.ToString();
        }

        private void AppendAssemblySetup(StringBuilder sb, ISessionContext sessionContext)
        {
            sb.AppendLine("   var eh = new StorEvil.Interpreter.ExtensionMethodHandler();");
            sb.AppendLine("   // _sessionContext = new SessionContext();");
            foreach (var assembly in sessionContext.GetAllAssemblies())
            {
                var assemblyRef = "typeof(" + assembly.GetTypes().First().FullName + ").Assembly";
                var assemblyLocation =assemblyRef + ".Location";
                sb.AppendLine("    eh.AddAssembly(" + assemblyRef + ");");
                sb.AppendLine(@"    StorEvil.Interpreter.ParameterConverters.ParameterConverter.AddCustomConverters(" + assemblyLocation + @");");
            }
        }
    }
}