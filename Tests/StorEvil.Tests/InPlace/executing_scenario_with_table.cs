using NUnit.Framework;
using StorEvil.Parsing;


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
Given the following table
|1|one|
|2|two|
|3|three|

table row 0 column 0 Should Be 1
table row 0 column 1 should be one

table row 1 column 0 should be 2

table row 1 column 1 should be two
table row 2 column 0 should be 3

table row 2 column 1 should be three
";

        [SetUp]
        public void SetupContext()
        {
            RunStory(new StoryParser().Parse(storyText, null));
        }

        [Test]
        public void is_successful()
        {
            AssertScenarioSuccess();           
        }

        [Test]
        public void no_failures()
        {
            AssertAllScenariosSucceeded();
        }

        [Context]
        public class ScenarioTableTestContext
        {
            public ScenarioTableTestContext()
            {
                Table = null;
            }

            private string[][] Table;

            public void Given_the_following_table(string[][] tableData)
            {
                Table = tableData;
            }

            public string Table_row_r_column_c(int r, int c)
            {
                return Table[r][c];
            }
        }
    }

    public class TestRow
    {
        public int IntField;
        public string StringProp { get; set; }
    }
}