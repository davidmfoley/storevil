using System.Linq;
using System.Text;
using StorEvil.CodeGeneration;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    /// <summary>
    /// Generates NUnit fixtures
    /// </summary>
    public class NUnitFixtureGenerator : IFixtureGenerator
    {

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
            sb.AppendLine("    var assemblyRegistry = new " + typeof(AssemblyRegistry).FullName +"( new System.Reflection.Assembly[] {");
            var asses = sessionContext.GetAllAssemblies().Select(x=> "typeof(" + x.GetTypes().First().FullName + ").Assembly");
            var joined = string.Join(",\r\n        ", asses.ToArray());

            sb.AppendLine(joined);
            
            sb.AppendLine("    });");

            sb.AppendLine("   var eh = new StorEvil.Interpreter.ExtensionMethodHandler(assemblyRegistry);");
            sb.AppendLine("   // _sessionContext = new SessionContext();");
        }
    }
}