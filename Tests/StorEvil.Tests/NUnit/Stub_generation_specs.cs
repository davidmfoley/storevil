using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.StubGeneration;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
    [TestFixture]
    public class Stub_generation_specs
    {
        private StubGenerator Generator;
        private Story TestStory;
        private string Suggestions;
        private ITextWriter FakeWriter;
        private IAmbiguousMatchResolver FakeResolver;

        [SetUp]
        public void SetupContext()
        {
            FakeWriter = MockRepository.GenerateMock<ITextWriter>();
            FakeResolver = MockRepository.GenerateMock<IAmbiguousMatchResolver>();

            Generator =
                new StubGenerator(new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler(new AssemblyRegistry())), FakeResolver),
                                  new ImplementationHelper(), FakeWriter, new FakeSessionContext());

            TestStory = new Story("foo", "", new[]
                                                 {
                                                      TestHelper.BuildScenario("", new[] { "first line", "second line" }),
                                                      TestHelper.BuildScenario("", new[] { "first line", "second line", "third line" }),
                                                      TestHelper.BuildScenario("", new[] { "this line's weird, punctuation! should be: handled"})
                                                 });

            Generator.HandleStory(TestStory);
            Generator.Finished();

            Suggestions = (string) FakeWriter.GetArgumentsForCallsMadeOn(x => x.Write(Arg<string>.Is.Anything))[0][0];            
        }

        [Test]
        public void Should_suggest_methods()
        {
            Suggestions.ShouldContain("first_line()");
            Suggestions.ShouldContain("second_line()");
        }

        [Test]
        public void Should_only_suggest_each_method_once()
        {
            Suggestions.ShouldNotMatch("first_line\\(\\).+first_line\\(");           
        }

        [Test]
        public void Generated_code_should_compile()
        {
            CreateTestAssembly(Suggestions).ShouldNotBeNull();
        }

        private static Assembly CreateTestAssembly(string code)
        {
            const string header = "using System;\r\n";
            return TestHelper.CreateAssembly(header + "\r\n" + code);
        }
    }
}