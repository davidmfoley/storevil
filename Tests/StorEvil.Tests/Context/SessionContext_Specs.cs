using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Utility;

namespace StorEvil.Context
{   
    [TestFixture]
    public class SessionContext_Specs
    {
        [Test]
        public void Should_Map_By_ContextAttribute()
        { 
            var mapper = new SessionContext();
            mapper.AddContext<TestMappingContext>();

            var context = mapper.GetContextForStory();
            context.ImplementingTypes.First().ShouldEqual(typeof (TestMappingContext));
        }

        [Test]
        public void Should_Register_Assembly_Types()
        {
            var mapper = new SessionContext();
            mapper.AddAssembly(GetType().Assembly);

            var context = mapper.GetContextForStory();
            context.ImplementingTypes.ShouldContain(typeof (TestMappingContext));
        }

        [Test, Ignore]
        public void Returns_a_default_context_with_only_object_if_no_contexts_added()
        {
            var mapper = new SessionContext();

             var result = mapper.GetContextForStory();
            result.ImplementingTypes.Count().ShouldEqual(1);
        }
    }

    [Context]
    public class TestMappingContext
    {
    }

    
}