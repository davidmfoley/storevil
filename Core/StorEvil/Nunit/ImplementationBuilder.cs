using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.Nunit
{
    public class ImplementationBuilder
    {
        private readonly CSharpMethodInvocationGenerator _invocationGenerator;
        private string _previousSignificantFirstWord;

        public ImplementationBuilder(CSharpMethodInvocationGenerator invocationGenerator)
        {
            _invocationGenerator = invocationGenerator;
        }

        public TestImplementation BuildImplementation(Scenario scenario, StoryContext context)
        {
            var impl = new TestImplementation();

            foreach (var line in scenario.Body)
            {
                impl.CodeBuilder.Append(BuildConsoleWriteScenarioLine(line.Text));
                impl.CodeBuilder.AppendLine("#line " + line.LineNumber);
                var lineVariations = GenerateLineVariations(line.Text);

                ScenarioLineImplementation functionLine = null;
                foreach (var variation in lineVariations)
                {
                    if (null != (functionLine = BuildCodeFromScenarioLine(variation, context)))
                        break;
                }

                if (functionLine == null)
                {
                    impl.CodeBuilder.AppendLine(
                        @"            Assert.Ignore(@""Could not interpret: '" + line.Text + "'\");");
                    impl.CodeBuilder.AppendLine("#line hidden");
                    break;
                }

                impl.Contexts.Add(functionLine.Context);

                impl.CodeBuilder.AppendLine(functionLine.Code);
                impl.CodeBuilder.AppendLine("#line hidden");
                impl.Namespaces = impl.Namespaces.Union(functionLine.Namespaces).Distinct();
            }

            AppendDisposeCalls(impl.CodeBuilder, impl.Contexts);
            return impl;
        }

        private ScenarioLineImplementation BuildCodeFromScenarioLine(string line, StoryContext storyContext)
        {
            var scenarioContext = storyContext.GetScenarioContext();

            LineInfo invocation = null;

            invocation = _invocationGenerator.MapMethod(scenarioContext, line);
            if (invocation == null)
                return null;

            var chosenType = invocation.ImplementingType;

            return new ScenarioLineImplementation("context" + chosenType.Name + invocation.Code + ";", chosenType,
                                                  "context" + chosenType.Name, invocation.Namespaces);
        }

        private string BuildConsoleWriteScenarioLine(string line)
        {
            return string.Format("           System.Console.WriteLine(@\"{0}\");\r\n", line.Replace("\"", "\"\""));
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

        private void AppendDisposeCalls(StringBuilder codeBuilder, TestContextSet contexts)
        {
            foreach (var context in contexts.Where(c => c.Type.GetInterfaces().Contains(typeof (IDisposable))))
            {
                codeBuilder.AppendLine("            " + context.Name + ".Dispose();");
            }
        }
    }

    public class TestImplementation
    {
        public StringBuilder CodeBuilder = new StringBuilder();
        public TestContextSet Contexts = new TestContextSet();
        public IEnumerable<string> Namespaces = new string[0];
    }
}