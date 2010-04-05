using NUnit.Framework;
using StorEvil.InPlace;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class executing_scenario_with_table
        : InPlace.executing_scenario_with_table, UsingCompiledRunner
    {
    }
}


namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class executing_scenario_with_table
        : InPlace.executing_scenario_with_table, UsingNonCompiledRunner
    {
    }
}

namespace StorEvil.InPlace
{
    public abstract  class executing_scenario_with_table :
        InPlaceRunnerSpec<executing_scenario_with_table.ScenarioTableTestContext>
    {
        private string storyText =
            @"
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
            RunStory(new StoryParser().Parse(storyText, null));
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

    public class TestRow
    {
        public int IntField;
        public string StringProp { get; set; }
    }
}