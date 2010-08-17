using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class executing_outlines_with_example_tables
        : InPlace.executing_outlines_with_example_tables, UsingCompiledRunner { }

}

namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class executing_outlines_with_example_tables
        : InPlace.executing_outlines_with_example_tables, UsingNonCompiledRunner { }

}
namespace StorEvil.InPlace
{

    public abstract class executing_outlines_with_example_tables : InPlaceRunnerSpec<InPlaceRunnerTableTestContext>
    {
        private string storyText =
            @"
Story: test 

Scenario Outline:
Call a method with <int> and <string>
An example should have <int> and <string>

Examples:
| int | string |
| 1   | one    |
| 2   | two    |
| 3   | three  |
";

        [SetUp]
        public void SetupContext()
        {                    
            var story = new StoryParser().Parse(storyText, null);
            RunStory(story);
        }
        
        [Test]
        public void should_be_successful()
        {
            AssertScenarioSuccess();            
        }

        [Test]
        public void should_be_no_failures()
        {
            AssertAllScenariosSucceeded();
        }
    }

    [Context]
    public class InPlaceRunnerTableTestContext
    {
        private List<Call> Calls = new List<Call>();

        public virtual void Call_a_method_with_intParam_and_stringParam(int intParam, string stringParam)
        {
            Calls.Add(new Call(intParam, stringParam));
        }

        public void An_example_should_have_intParam_and_stringParam(int intParam, string stringParam)
        {
            var call = Calls[0];
            call.IntParam.ShouldEqual(intParam);
            call.StringParam.ShouldEqual(stringParam);
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