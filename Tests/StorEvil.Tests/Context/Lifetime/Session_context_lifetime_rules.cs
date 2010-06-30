using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Utility;

namespace StorEvil.Context.Lifetime
{
    [TestFixture]
    public class Session_context_lifetime_rules
    {
        private readonly Type TestContextType = typeof(TestSessionLifetimeMappingContext);
        private SessionContext Mapper;

       
        [SetUp]
        public void SetupContext()
        {
            Mapper = new SessionContext();
            Mapper.AddContext<TestSessionLifetimeMappingContext>();
        }

        [Test]
        public void Context_classes_are_reused_within_different_scenarios_in_the_same_story()
        {
            var storyContext = GetStoryContext();

            var context1 = storyContext.GetScenarioContext().GetContext(TestContextType);
            var context2 = storyContext.GetScenarioContext().GetContext(TestContextType);

            Assert.That(context1, Is.SameAs(context2));
        }

        private StoryContext GetStoryContext()
        {
            return Mapper.GetContextForStory();
        }

        [Test]
        public void Context_classes_are_reused_within_different_stories()
        {
            var context1 = GetStoryContext().GetScenarioContext().GetContext(TestContextType);
            var context2 = GetStoryContext().GetScenarioContext().GetContext(TestContextType);

            Assert.That(context1, Is.SameAs(context2));
        }

        [Test]
        public void Context_class_is_disposed_when_Session_ends()
        {
            GetStoryContext().GetScenarioContext().GetContext(TestContextType);
            Mapper.Dispose();
            TestSessionLifetimeMappingContext.WasDisposed.ShouldEqual(true);
        }
    }
}