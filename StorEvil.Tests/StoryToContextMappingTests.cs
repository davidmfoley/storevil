using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
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
            var story = new Story("context test", "context test", new List<IScenario>());

            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();

            var context = mapper.GetContextForStory(story);
            context.ImplementingTypes.First().ShouldEqual(typeof (TestMappingContext));
        }

        [Test]
        public void Should_Register_Assembly_Types()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddAssembly(GetType().Assembly);

            var context = mapper.GetContextForStory(new Story("context test", "context test", new List<IScenario>()));
            context.ImplementingTypes.First().ShouldEqual(typeof (TestMappingContext));
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

    [TestFixture]
    public class Context_lifetime_rules
    {
        private StoryContext StoryContext;

        [SetUp]
        public void SetupContext()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();
            StoryContext = mapper.GetContextForStory(new Story("", "", new IScenario[] {}));
        }

        [Test]
        public void Context_classes_are_reused_within_one_scenario()
        {
            var context = StoryContext.GetScenarioContext();

            var context1 = context.GetContext(typeof (TestMappingContext));
            var context2 = context.GetContext(typeof (TestMappingContext));

            Assert.That(context1, Is.SameAs(context2));
        }

        [Test]
        public void Context_classes_for_different_scenarios_are_different_objects()
        {
            var context1 = StoryContext.GetScenarioContext().GetContext(typeof (TestMappingContext));
            var context2 = StoryContext.GetScenarioContext().GetContext(typeof (TestMappingContext));

            Assert.That(context1, Is.Not.SameAs(context2));
        }

        [Test]
        public void Disposable_context_classes_are_disposed_at_the_end_of_the_scenario()
        {
            DisposableMappingContext d;
            using (var context = StoryContext.GetScenarioContext())
            {
                d = (DisposableMappingContext) context.GetContext(typeof (DisposableMappingContext));
            }
            d.WasDisposed.ShouldEqual(true);
        }
    }

    [TestFixture]
    public class Dependent_contexts
    {
        private StoryContext StoryContext;

        [SetUp]
        public void SetupContext()
        {
            var mapper = new StoryToContextMapper();
            mapper.AddContext<TestMappingContext>();
            mapper.AddContext<DependentMappingContext>();
            StoryContext = mapper.GetContextForStory(new Story("", "", new IScenario[] {}));
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
            var dependedOn = (DisposableMappingContext)ScenarioContext.GetContext(typeof(DisposableMappingContext));
            var dependent = (DependentMappingContext)ScenarioContext.GetContext(typeof(DependentMappingContext));
            
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