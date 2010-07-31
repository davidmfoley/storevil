using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.NUnit
{

    [TestFixture]
    public class NUnitFixtureGeneratorTests : TestBase
    {
        [Test]
        public void Should_Create_One_Test_For_A_Single_Scenario()
        {
            var s = BuildScenario("foo", new[] {"When I Do Something"});
            var story = new Story("foobar", "summary", new[] {s});

            Assembly a = BuildTestAssembly(story);
            var fixture = a.GetTypes()[0];
            fixture.GetTestMethods().Count().ShouldEqual(1);
        }

        [Test]
        public void Should_Compile_When_No_Scenarios()
        {
            var s = new Story("test", "summary", new Scenario[] {});
            Assembly a = BuildTestAssembly(s);
        }

        [Test]
        public void Should_Contain_A_Single_testFixture_for_a_Story()
        {
            var s = new Story("test", "summary", new Scenario[] {});
            Assembly a = BuildTestAssembly(s);
            a.GetTypes().Length.ShouldEqual(1);
        }

        [Test]
        public void Should_have_category_for_each_tag_on_story()
        {
            var scenario = BuildScenario("test", new[] { "When I Do Something" });
            var s = new Story("foo", "summary", new Scenario[] { scenario }) { Tags = new[] { "foo", "bar" } };
            Assembly a = BuildTestAssembly(s);
            var testClass = a.GetTypes().First(x=>x.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any());
            var attributes = testClass
                .GetCustomAttributes(typeof (CategoryAttribute), true)
                .Cast<CategoryAttribute>()
                .Select(x=>x.Name);

            attributes.OrderBy(x=>x).ElementsShouldEqual("bar", "foo");
            
        }

        [Test]
        public void Should_name_class_after_id()
        {
            var s = BuildScenario("test", new[] { "When I Do Something" });
            var story = new Story("C:\\Example\\Path\\To\\File.feature", "summary", new[] { s });

            Assembly a = BuildTestAssembly(story);
            var fixture = a.GetTypes()[0];
            fixture.Name.ShouldContain("Example_Path_To_File");
        }

        private Assembly BuildTestAssembly(Story story)
        {

            var generator = new NUnitFixtureGenerator(new ScenarioPreprocessor());
            var code = generator.GenerateFixture(story, GetContext());

            return CreateTestAssembly(code);
        }

        private static Assembly CreateTestAssembly(string code)
        {
            const string header = "using System;\r\nusing NUnit.Framework;";
            return TestHelper.CreateAssembly(header + "\r\n" + code);
        }

       

        private StoryContext GetContext()
        {
            return new StoryContext(new FakeSessionContext(), new[] { typeof(TestContext) });
        }

        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            int i = 1;
            return new Scenario("test", lines.Select(line => new ScenarioLine { Text = line, LineNumber = ++i}).ToArray());
        }
    }
}