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

        /// <summary>
        /// For now, return object if no match and this way at least every test will throw
        /// </summary>
        [Test]
        public void Should_Return_Object_If_No_Match()
        {

            var mapper = new StoryToContextMapper();
           
            var context = mapper.GetContextForStory(new Story("unknown type", "totally bogus", new List<IScenario>()));
            context.ImplementingTypes.First().ShouldEqual(typeof(object));
        }
    }
}
