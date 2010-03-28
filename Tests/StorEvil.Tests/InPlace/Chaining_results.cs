using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Chaining_results : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "sub context property should be true";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_succeed()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }
    }

    [TestFixture]
    public class Filtering_by_tags
    {
        private TagFilter Filter;

        [SetUp]
        public void SetupContext()
        {
            Filter = new TagFilter(new [] {"foo", "bar"});
        }

        bool FilterIncludes(string[] storyTags, string[] scenarioTags)
        {
            var scenario = new Scenario() {Tags = storyTags};
            var story = new Story("", "", new[] { scenario }) { Tags = scenarioTags };

            return Filter.Include(story, scenario);
        }

        [Test]
        public void allows_story_with_tag()
        {
            FilterIncludes(new[] {"foo"}, new string[0]).ShouldEqual(true);
        }

        [Test]
        public void allows_scenario_with_tag()
        {
            FilterIncludes(new string[0], new [] {"bar"}).ShouldEqual(true);
        }

        [Test]
        public void does_not_include_when_neither_has_any_tags()
        {
            FilterIncludes(new string[0], new string[0] ).ShouldEqual(false);
        }

        [Test]
        public void does_not_include_when_tags_do_not_match()
        {
            FilterIncludes(new[] { "42", "foobar" }, new string[] { "baz", "foobar" }).ShouldEqual(false);
        
        }
    }
}