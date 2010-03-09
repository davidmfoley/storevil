using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class executing_outlines_with_example_tables : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private string storyText = @"
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
            Context = new StoryContext(typeof(InPlaceRunnerTableTestContext));

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor()).HandleStory(story, Context);
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



    [TestFixture]
    public class executing_scenario_with_table : InPlaceRunnerSpec<executing_scenario_with_table.ScenarioTableTestContext>
    {
        private string storyText = @"
Story: test tables in scenarios
Scenario:
Given the following
|1|one|
|2|two|
|3|three|
";

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var story = new StoryParser().Parse(storyText, null);
            Context = new StoryContext(typeof(ScenarioTableTestContext));

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor()).HandleStory(story, Context);           
        }

        [Test]
        public void Table_is_not_null()
        {
            ScenarioTableTestContext.Table.ShouldNotBeNull();
        }

        [Test]
        public void Table_data_is_set()
        {
            var table = ScenarioTableTestContext.Table;
            table.Length.ShouldEqual(3);
            table[0][0].ShouldEqual("1");
            table[0][1].ShouldEqual("one");
            
            table[1][0].ShouldEqual("2");
            table[1][1].ShouldEqual("two");

            table[2][0].ShouldEqual("3");
            table[2][1].ShouldEqual("three");

        }

        public class ScenarioTableTestContext
        {
            public ScenarioTableTestContext()
            {
                Table = null;
            }

            public static string[][] Table;

            public void Given_the_following(string[][] table)
            {
                Table = table;
            }
        }
    }

}