using System;
using System.Linq;
using System.Text;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.CodeGeneration
{
    public class TestFixtureClassGenerator
    {
        public string Generate(Story story, string defaultNamespace)
        {
            var stringBuilder = new StringBuilder();
            string fixtureName = GetFixtureName(story);

#if DEBUG
            System.Diagnostics.EventLog.WriteEntry("StorEvil", String.Concat("Generated Fixture Name: ", fixtureName));
#endif

            stringBuilder.Append("namespace " + defaultNamespace + "{");
            var categories = string.Join("", (story.Tags ?? new string[0]).Select(t => string.Format(@"[Category(""{0}"")]", t)).ToArray());
            stringBuilder.AppendLine(categories);
        
            stringBuilder.Append("\r\n[NUnit.Framework.TestFixtureAttribute] public class " + fixtureName + " : StorEvil.CodeGeneration.TestFixture {\r\n ");
            stringBuilder.Append("  public object Contexts { get { return base.GetContexts();}}");
            AddLifecycleHandlers(stringBuilder);
            
            AddScenarios(story, stringBuilder);
            stringBuilder.Append("  }\r\n}");

            return stringBuilder.ToString();
        }

        private string GetFixtureName(Story story)
        {
            if (!string.IsNullOrEmpty(story.Id))
                return story.Id.ToCSharpMethodName();
            return story.Summary.ToCSharpMethodName();
        }

        private void AddScenarios(Story story, StringBuilder stringBuilder)
        {
            foreach (var scenario in story.Scenarios)
            {
                var name = scenario.Name.ToCSharpMethodName();
                stringBuilder.AppendLine("  [NUnit.Framework.TestAttribute] public void " + name + "() {");
                stringBuilder.AppendFormat("#line 1  \"{0}\"\r\n#line hidden\r\n", story.Id);
                stringBuilder.AppendLine(GetBody(scenario) +  "  }");                
            }
        }

        private void AddLifecycleHandlers(StringBuilder stringBuilder)
        {
            AppendHandler(stringBuilder, "SetUp", "base.BeforeEach();");
            AppendHandler(stringBuilder, "TestFixtureSetUp", "SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll();");
            AppendHandler(stringBuilder, "TearDown", "base.AfterEach();");
            AppendHandler(stringBuilder, "TestFixtureTearDown", "base.AfterAll();");
        }

        private void AppendHandler(StringBuilder stringBuilder, string  name, string code)
        {
            stringBuilder.AppendFormat("    [NUnit.Framework.{0}Attribute]\r\n    public void Handle{0}() {{ {1} }}\r\n", name, code);
        }

        private string GetBody(IScenario scenario)
        {
            if (scenario is Scenario)
                return GetScenarioBody((Scenario) scenario);
            return null;
        }

        private string GetScenarioBody(Scenario scenario)
        {
            var sourceLines = (scenario.Background ?? new ScenarioLine[0]).Union(scenario.Body);

            var lines = sourceLines.Select((l, i) => GetScenarioLine(l));

            return string.Join("\r\n", lines.ToArray());
        }

        private string GetScenarioLine(ScenarioLine scenarioLine)
        {
            const string fmt = "#line {0}\r\nExecuteLine(@\"{1}\");\r\n#line hidden\r\n";
            return string.Format(fmt, scenarioLine.LineNumber, scenarioLine.Text.Replace("\"", "\"\""));
        }
    }
}