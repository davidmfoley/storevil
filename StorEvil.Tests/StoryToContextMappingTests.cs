using System.Configuration;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil
{
    [Context("context test")]
    public class TestMappingContext
    {
        
    }

    [TestFixture] 
    public class StoryToContextMappingTests
    {
        [Test]
        public void Should_Map_By_ContextAttribute()
        {

            var story = new Story("context test", "context test",  new List<IScenario>() );

            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();

            var context = mapper.GetContextForStory(story);
            context.ImplementingTypes.First().ShouldEqual(typeof(TestMappingContext));    
        }

        [Test]
        public void Should_Register_Assembly_Types()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddAssembly(GetType().Assembly);

            var context = mapper.GetContextForStory(new Story("context test", "context test", new List<IScenario>() ));
            context.ImplementingTypes.First().ShouldEqual(typeof(TestMappingContext));    
        }

        [Test]
        public void Throws_if_no_context_added()
        {

            var mapper = new StoryToContextMapper();

            Expect.ThisToThrow<ConfigurationException>(() => mapper.GetContextForStory(new Story("unknown type",
                                                                                                 "totally bogus",
                                                                                                 new List<IScenario>())));
        }

        [Test]
        public void Context_classes_are_reused_within_one_scenario()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();

            var storyContext = mapper.GetContextForStory(new Story("", "", new IScenario[] { }));

            var context = storyContext.GetScenarioContext();

            var context1 = context.GetContext(typeof(TestMappingContext));
            var context2 = context.GetContext(typeof(TestMappingContext));

            Assert.That(context1, Is.SameAs(context2));
        }

        [Test]
        public void Context_classes_for_different_scenarios_are_different_objects()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();

            var storyContext = mapper.GetContextForStory(new Story("", "", new IScenario[] {}));

            var context1 = storyContext.GetScenarioContext().GetContext(typeof (TestMappingContext));
            var context2 = storyContext.GetScenarioContext().GetContext(typeof (TestMappingContext));

            Assert.That(context1, Is.Not.SameAs(context2));
        }
    }
}
