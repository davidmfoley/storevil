using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Nunit
{
    /// <summary>
    /// Generates NUnit source code for scenarios
    /// 
    /// TODO: this is sort of POC code... needs a 2nd look
    /// </summary>
    public class NUnitTestMethodGenerator
    {
        private readonly CSharpMethodInvocationGenerator _invocationGenerator;
        private string _previousSignificantFirstWord;

        public NUnitTestMethodGenerator()
        {
            //TODO
            _invocationGenerator =
                new CSharpMethodInvocationGenerator(
                    new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler())));
        }

        public NUnitTestMethodGenerator(CSharpMethodInvocationGenerator invocationGenerator)
        {
            _invocationGenerator = invocationGenerator;
        }

        public virtual NUnitTest GetTestFromScenario(Scenario scenario, StoryContext context)
        {
            var codeBuilder = new StringBuilder();

            var contexts = new TestContextSet();

            IEnumerable<string> namespaces = new string[0];
            foreach (var line in scenario.Body)
            {
                codeBuilder.Append(BuildConsoleWriteScenarioLine(line.Text));

                var lineVariations = GenerateLineVariations(line.Text);

                ScenarioLineImplementation functionLine = null;
                foreach (var variation in lineVariations)
                {
                    if (null != (functionLine = BuildCodeFromScenarioLine(variation, context)))
                        break;
                }

                if (functionLine == null)
                {
                    codeBuilder.AppendLine(
                        @"            Assert.Ignore(@""Could not interpret: '" + line + "'\");");
                    break;
                }

                contexts.Add(functionLine.Context);
                codeBuilder.AppendLine(functionLine.Code);

                namespaces = namespaces.Union(functionLine.Namespaces).Distinct();
            }

            
            AppendDisposeCalls(codeBuilder, contexts);

            string declarations = BuildContextDeclarations(contexts);
            var name = BuildMethodName(scenario);
            var categories = BuildCategories(scenario);

            var body = BuildTestBody(codeBuilder, name, declarations, categories);

            return new NUnitTest(name, body, contexts, namespaces);
        }

        private string BuildCategories(Scenario scenario)
        {
            if (scenario.Tags == null || scenario.Tags.Count() == 0)
                return "";
            return string.Join("", scenario.Tags.Select(t => string.Format(@"[Category(""{0}"")]", t)).ToArray());
        }

        private void AppendDisposeCalls(StringBuilder codeBuilder, TestContextSet contexts)
        {
            foreach (var context in contexts.Where(c => c.Type.GetInterfaces().Contains(typeof (IDisposable))))
            {
                codeBuilder.AppendLine("            " + context.Name + ".Dispose();");
            }
        }

        private IEnumerable<string> GenerateLineVariations(string line)
        {
            var firstWordIsAnd = FirstWordIsAnd(line);
            if (!firstWordIsAnd)
                _previousSignificantFirstWord = GetFirstWord(line);

            yield return line;

            if (firstWordIsAnd && _previousSignificantFirstWord != null)
                yield return _previousSignificantFirstWord + line.Substring(line.IndexOf(" "));
        }

        private string GetFirstWord(string line)
        {
            return line.Split(' ').FirstOrDefault();
        }

        private bool FirstWordIsAnd(string line)
        {
            return GetFirstWord(line).ToLower() == "and";
        }

        private string BuildConsoleWriteScenarioLine(string line)
        {
            return string.Format("           System.Console.WriteLine(@\"{0}\");\r\n", line.Replace("\"", "\"\""));
        }

        private string BuildTestBody(StringBuilder codeBuilder, string name, string declarations, string categories)
        {
            return string.Format("[Test]{3} public void {0}(){{\r\n{1}\r\n{2}        }}", name, declarations, codeBuilder, categories);
        }

        private static string BuildContextDeclarations(IEnumerable<TestContextField> contexts)
        {
            var declarationLines =
                contexts.Select(x => string.Format("            var {0} = new {1}();", x.Name, x.Type.FullName));
            return string.Join("\r\n", declarationLines.ToArray());
        }

        private static string BuildMethodName(Scenario scenario)
        {
            string baseName = GetBaseName(scenario);

            var name = StripNonNameCharacters(baseName.Trim());

            if (name.Length > 254)
                name = name.Substring(0, 254);

            // just make up a random name for now
            if (name.Length == 0)
                return "_" + Guid.NewGuid().ToString().Replace("-", "");

            // make sure it starts with letter, else prepend _
            if (char.IsLetter(name[0]))
                return name;

            return "_" + name;
        }

        private static string StripNonNameCharacters(string baseName)
        {
            var name = new StringBuilder();

            foreach (char c in baseName.Replace("_", " ").Trim())
            {
                if (CanBeUsedInCSharpName(c))
                    name.Append(c);
                else
                    name.Append("_");
            }

            return name.ToString();
        }

        private static bool CanBeUsedInCSharpName(char c)
        {
            return Char.IsLetter(c) || Char.IsNumber(c);
        }

        private static string GetBaseName(Scenario scenario)
        {
            var baseName = scenario.Name;

            if (string.IsNullOrEmpty(scenario.Name))
            {
                baseName = "";
                foreach (var s in scenario.Body)
                    baseName += s.Text + "_";
            }
            return baseName;
        }

        private ScenarioLineImplementation BuildCodeFromScenarioLine(string line, StoryContext storyContext)
        {
            var scenarioContext = storyContext.GetScenarioContext();
            
            LineInfo invocation = null;
            
            invocation = _invocationGenerator.MapMethod(scenarioContext, line);
            if (invocation == null)
                return null;

            var chosenType = invocation.ImplementingType;

            return new ScenarioLineImplementation("            context" + chosenType.Name + invocation.Code + ";", chosenType,
                                                  "context" + chosenType.Name, invocation.Namespaces);
        }
    }
}