using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Assertions;
using StorEvil.Utility;

namespace StorEvil.Context.Lifetime
{
    [TestFixture]
    public class Story_context_lifetime_rules
    {
       

        private readonly Type TestContextType = typeof(TestStoryLifetimeMappingContext);
        private SessionContext Mapper;

        [SetUp]
        public void SetupContext()
        {
            Mapper = new SessionContext();
            Mapper.AddContext<TestStoryLifetimeMappingContext>();            
        }

        [Test]
        public void Context_classes_are_reused_within_different_scenarios_in_the_same_story()
        {
            var storyContext = GetStoryContext();           

            var context1 = GetScenarioContext(storyContext);
            var context2 = GetScenarioContext(storyContext);           

            Assert.That(context1, Is.SameAs(context2));
        }

        [Test]
        public void Context_classes_are_not_reused_within_different_stories()
        {
            var context1 = GetScenarioContext(GetStoryContext());
            var context2 = GetScenarioContext(GetStoryContext());
            
            Assert.That(context1, Is.Not.SameAs(context2));
        }

        [Test]
        public void Context_is_disposed_at_the_end_of_the_story()
        {
            var storyContext = GetStoryContext();
            GetScenarioContext(storyContext);

            storyContext.Dispose();
      
            TestStoryLifetimeMappingContext.WasDisposed.ShouldEqual(true);           
        }

        private object GetScenarioContext(StoryContext storyContext)
        {
            return storyContext.GetScenarioContext().GetContext(TestContextType);
        }

        private StoryContext GetStoryContext()
        {
            return Mapper.GetContextForStory();
        }
    }
}