using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Utility;

namespace StorEvil.Context
{
    [TestFixture]
    public class Dependent_contexts
    {
        private StoryContext StoryContext;

        [SetUp]
        public void SetupContext()
        {
            var mapper = new SessionContext();
            mapper.AddContext<TestMappingContext>();
            mapper.AddContext<DependentMappingContext>();
            StoryContext = mapper.GetContextForStory();
            ScenarioContext = StoryContext.GetScenarioContext();
        }

        protected ScenarioContext ScenarioContext { get; set; }

        [Test]
        public void Dependent_class_is_created()
        {
            var dependent = (DependentMappingContext) ScenarioContext.GetContext(typeof (DependentMappingContext));
            dependent.DependsOn.ShouldNotBeNull();
        }

        [Test]
        public void Dependent_object_is_same_as_explicitly_resolved_object()
        {
            var dependedOn = (DisposableMappingContext) ScenarioContext.GetContext(typeof (DisposableMappingContext));
            var dependent = (DependentMappingContext) ScenarioContext.GetContext(typeof (DependentMappingContext));

            Assert.That(dependent.DependsOn, Is.SameAs(dependedOn));
        }
    }

    public class DisposableMappingContext : IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public class DependentMappingContext
    {
        private readonly DisposableMappingContext _dependsOn;

        public DependentMappingContext(DisposableMappingContext dependsOn)
        {
            _dependsOn = dependsOn;
        }

        public DisposableMappingContext DependsOn
        {
            get { return _dependsOn; }
        }

        public void Dependent()
        {
        }
    }
}