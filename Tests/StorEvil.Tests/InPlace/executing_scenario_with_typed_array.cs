using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class executing_scenario_with_typed_array :
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
";

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var story = new StoryParser().Parse(storyText, null);
            Context = new StoryContext(typeof(ScenarioArrayTestContext));

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()))).HandleStory(story, Context);
        }

        [Test]
        public void Table_is_not_null()
        {
            ScenarioArrayTestContext.Table.ShouldNotBeNull();
        }

        [Test]
        public void Table_data_is_set()
        {
            var table = ScenarioArrayTestContext.Table;
            table.Length.ShouldEqual(3);
            table[0].IntField.ShouldEqual(1);
            table[1].IntField.ShouldEqual(2);
            table[2].IntField.ShouldEqual(3);
            table[0].StringProp.ShouldEqual("one");
            table[1].StringProp.ShouldEqual("two");
            table[2].StringProp.ShouldEqual("three");

        }

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
        }
    }

    [TestFixture]
    public class executing_scenario_with_typed_parameter :
        InPlaceRunnerSpec<executing_scenario_with_typed_parameter.TypedScenarioTestContext>
    {
        private string storyText =
            @"
Story: test tables in scenarios
Scenario:
Given the following
|IntField|42|
|StringProp|foobar|
";

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var story = new StoryParser().Parse(storyText, null);
            Context = new StoryContext(typeof(TypedScenarioTestContext));

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()))).HandleStory(story, Context);

        }

        [Test]
        public void Table_is_not_null()
        {
            TypedScenarioTestContext.Row.ShouldNotBeNull();
        }

        [Test]
        public void Table_data_is_set()
        {
            var row = TypedScenarioTestContext.Row;
           row.StringProp.ShouldEqual("foobar");
            row.IntField.ShouldEqual(42);

        }

        public class TypedScenarioTestContext
        {
            public TypedScenarioTestContext()
            {
                Row = null;
            }
           
            public static TestRow Row;

            public void Given_the_following(TestRow row)
            {
                Row = row;
            }
        }
    }
}