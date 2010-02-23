using System.Configuration;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
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
    }
}
