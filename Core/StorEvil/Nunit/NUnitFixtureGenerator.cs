using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.NUnit
{

    public class StorEvilTestFixtureBase
    {
        private Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        protected void SetUp()
        {
            
        }

        protected void CleanUpScenario()
        {
            var typesToCleanup = _cache.Keys.Where(IsScenarioLifetime) ?? new Type[0];

            DestroyContexts(typesToCleanup);
        }

        protected void CleanUpStory()
        {
            var typesToCleanup = _cache.Keys.Where(IsStoryLifetime);

            DestroyContexts(typesToCleanup);
        }

        private void DestroyContexts(IEnumerable<Type> typesToCleanup)
        {
            foreach (var type in typesToCleanup.ToArray())
            {
                var context = _cache[type];
                if (context is IDisposable)
                    ((IDisposable)context).Dispose();

                _cache.Remove(type);
            }
        }

        private bool IsScenarioLifetime(Type type)
        {
            return GetLifetime(type) == ContextLifetime.Scenario;
        }

        private ContextLifetime GetLifetime(Type type)
        {
            var attribute = (StorEvil.ContextAttribute)type.GetCustomAttributes(typeof (StorEvil.ContextAttribute), true).FirstOrDefault();
            if (attribute == null)
                return ContextLifetime.Scenario;

            return attribute.Lifetime;
        }

        private bool IsStoryLifetime(Type type)
        {
            return GetLifetime(type) != ContextLifetime.Scenario;
        }

        protected T GetContext<T>()
        {
            var type = typeof (T);
            if (!_cache.ContainsKey(type))
                _cache.Add(type, Activator.CreateInstance(type));

            return (T)_cache[type];
        }
        
    }
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

            sb.AppendLine("namespace StorEvilSpecifications { [SetUpFixture] public class SetupAndTearDown {");
            sb.AppendLine("  [SetUp] public void SetUp() {");
            
            sb.AppendLine();
            sb.AppendLine("  }");
            sb.AppendLine("  [TearDown] public void TearDown() {");
            sb.AppendLine("  }");
            sb.AppendLine("} }");
            return sb.ToString();
        }
    }
}