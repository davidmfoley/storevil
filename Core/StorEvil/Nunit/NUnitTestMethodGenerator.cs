using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.Nunit
{
    public interface ITestMethodGenerator
    {
        NUnitTest GetTestFromScenario(Scenario scenario, StoryContext context);
    }

    /// <summary>
    /// Generates NUnit source code for scenarios
    /// </summary>
    public class NUnitTestMethodGenerator : ITestMethodGenerator
    {
        private readonly ImplementationBuilder _implementationBuilder;

        public NUnitTestMethodGenerator(CSharpMethodInvocationGenerator invocationGenerator)
        {
            _implementationBuilder = new ImplementationBuilder(invocationGenerator);
        }

        public virtual NUnitTest GetTestFromScenario(Scenario scenario, StoryContext context)
        {
            TestImplementation impl = _implementationBuilder.BuildImplementation(scenario, context);

            string declarations = BuildContextDeclarations(impl.Contexts);
            var name = BuildMethodName(scenario);
            var categories = BuildCategories(scenario);

            var body = BuildTestBody(impl.CodeBuilder, name, declarations, categories);

            return new NUnitTest(name, body, impl.Contexts, impl.Namespaces);
        }

        private string BuildCategories(Scenario scenario)
        {
            if (scenario.Tags == null || scenario.Tags.Count() == 0)
                return "";
            return string.Join("", scenario.Tags.Select(t => string.Format(@"[Category(""{0}"")]", t)).ToArray());
        }

        private string BuildTestBody(StringBuilder codeBuilder, string name, string declarations, string categories)
        {
            return string.Format("[Test]{3} public void {0}(){{\r\n{1}\r\n{2}\r\n\r\n        }}", name, declarations,
                                 codeBuilder, categories);
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
    }
}