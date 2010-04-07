using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class executing_scenario_with_typed_array
        : InPlace.executing_scenario_with_typed_array, UsingCompiledRunner
    {
    }

    [TestFixture]
    public class executing_scenario_with_typed_parameter
        : InPlace.executing_scenario_with_typed_parameter, UsingCompiledRunner
    {
    }
}


namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class executing_scenario_with_typed_array
        : InPlace.executing_scenario_with_typed_array, UsingNonCompiledRunner
    {
    }

    [TestFixture]
    public class executing_scenario_with_typed_parameter
        : InPlace.executing_scenario_with_typed_parameter, UsingNonCompiledRunner
    {
    }
}

namespace StorEvil.InPlace
{
    public abstract class executing_scenario_with_typed_array :
        InPlaceRunnerSpec<executing_scenario_with_typed_array.ScenarioArrayTestContext>
    {
        private string storyText =
            @"
Story: test tables in scenarios
Scenario:
Given the following
|IntField|StringProp|
|1|one|
|2|two|
|3|three|
typed params should include 1 one
typed params should include 2 two
typed params should include 3 three
";

        [SetUp]
        public void SetupContext()
        {
            var story = new StoryParser().Parse(storyText, null);
            RunStory(story);
        }

        [Test]
        public void should_succeed()
        {
            AssertScenarioSuccess();
        }

        [Context]
        public class ScenarioArrayTestContext
        {
            public ScenarioArrayTestContext()
            {
                Table = null;
            }

            public static TestRow[] Table;

            public void Given_the_following(TestRow[] table)
            {
                Table = table;
            }

            public void typed_params_should_include_i_s(int i, string s)
            {
                Table.Any(t=>t.IntField ==i && t.StringProp ==s).ShouldEqual(true);
            }
        }
    }

    public abstract class executing_scenario_with_typed_parameter :
        InPlaceRunnerSpec<executing_scenario_with_typed_parameter.TypedScenarioTestContext>
    {
        private string storyText =
            @"
Story: test tables in scenarios
Scenario:
Given the following row
|IntField|42|
|StringProp|foobar|
then the fields should be 42 and foobar
";

        [SetUp]
        public void SetupContext()
        {
            
            var story = new StoryParser().Parse(storyText, null);
            RunStory(story);
        }

        [Test]
        public void should_succeed()
        {
            AssertScenarioSuccess();
        }
      
        [Context]
        public class TypedScenarioTestContext
        {
            public TypedScenarioTestContext()
            {
                Row = null;
            }

            public static TestRow Row;

            public void Given_the_following_row(TestRow r)
            {
                Row = r;
            }

            public void then_the_fields_should_be_i_and_s(int i, string s)
            {
                Row.IntField.ShouldEqual(i);
                Row.StringProp.ShouldEqual(s);
            }
        }
    }
}