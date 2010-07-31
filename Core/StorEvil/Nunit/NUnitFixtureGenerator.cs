using System.Linq;
using System.Text;
using StorEvil.CodeGeneration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.NUnit
{
    /// <summary>
    /// Generates NUnit fixtures
    /// </summary>
    public class NUnitFixtureGenerator : IFixtureGenerator
    {
        private readonly IScenarioPreprocessor _preprocessor;

        public NUnitFixtureGenerator(IScenarioPreprocessor preprocessor)
        {
            _preprocessor = preprocessor;
        }

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

        public string GenerateFixture(Story story, StoryContext context)
        {
            return new TestFixtureClassGenerator().Generate(story, "StorEvilSpecs");
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
            sb.AppendLine("      StorEvil.CodeGeneration.TestSession.EndSession();");
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
                sb.AppendLine("    StorEvil.CodeGeneration.TestSession.AddAssembly(" + assemblyRef + ");");
                sb.AppendLine("    eh.AddAssembly(" + assemblyRef + ");");
                sb.AppendLine(@"    StorEvil.Interpreter.ParameterConverters.ParameterConverter.AddCustomConverters(" + assemblyLocation + @");");
            }
        }
    }
}