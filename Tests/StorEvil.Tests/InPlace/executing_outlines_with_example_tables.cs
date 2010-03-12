using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class executing_outlines_with_example_tables : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private string storyText =
            @"
Story: test 

Scenario Outline:
Call a method with <int> and <string>

Examples:
|int|string|
|1|one|
|2|two|
|3|three|
";

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var story = new StoryParser().Parse(storyText, null);
            Context = new StoryContext(typeof (InPlaceRunnerTableTestContext));

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()))).HandleStory(story, Context);
        }

        [Test]
        public void should_invoke_method_three_times()
        {
            InPlaceRunnerTableTestContext.Calls.Count.ShouldBe(3);
        }

        [Test]
        public void should_pass_correct_args()
        {
            ShouldBeACallWith(1, "one");
            ShouldBeACallWith(2, "two");
            ShouldBeACallWith(3, "three");
        }

        private void ShouldBeACallWith(int i, string s)
        {
            InPlaceRunnerTableTestContext.Calls.Any(c => c.IntParam == i && c.StringParam == s).ShouldEqual(true);
        }
    }

    public class InPlaceRunnerTableTestContext
    {
        public static List<Call> Calls = new List<Call>();

        public virtual void Call_a_method_with_intParam_and_stringParam(int intParam, string stringParam)
        {
            Calls.Add(new Call(intParam, stringParam));
        }

        public class Call
        {
            public int IntParam { get; set; }
            public string StringParam { get; set; }

            public Call(int intParam, string stringParam)
            {
                IntParam = intParam;
                StringParam = stringParam;
            }
        }
    }
}