using System;
using System.Collections.Generic;
using NUnit.Framework;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.Context
{
    [TestFixture]
    public class Debugging_contexts
    {
        private object View;
        private TestMappingContext _testMappingContext;

        [SetUp]
        public void SetupContext()
        {
            var viewer = new ContextViewer();
            var dictionary = new Dictionary<Type, object>();

            _testMappingContext = new TestMappingContext();
            dictionary.Add(typeof(TestMappingContext), _testMappingContext);

            View = viewer.Create(dictionary);
        }
        [Test]
        public void should_have_context_on_type()
        {            
            View.GetType().GetProperty("TestMappingContext").ShouldNotBeNull();
        }

        [Test]
        public void should_have_property_set()
        {          
            View.ReflectionGet("TestMappingContext").ShouldBe(_testMappingContext);
        }
    }
}