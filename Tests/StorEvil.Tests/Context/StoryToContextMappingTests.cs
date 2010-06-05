using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.Context.StoryContextFactory_Specs
{
    

    [TestFixture]
    public class StoryToContextMappingTests
    {
       

        [Test]
        public void Should_Map_By_ContextAttribute()
        {
            var story = new Story("context test", "context test", new List<IScenario>());

            var mapper = new SessionContext();
            mapper.AddContext<TestMappingContext>();

            var context = mapper.GetContextForStory(story);
            context.ImplementingTypes.First().ShouldEqual(typeof (TestMappingContext));
        }

        [Test]
        public void Should_Register_Assembly_Types()
        {
            var mapper = new SessionContext();
            mapper.AddAssembly(GetType().Assembly);

            var context = mapper.GetContextForStory(new Story("context test", "context test", new List<IScenario>()));
            context.ImplementingTypes.ShouldContain(typeof (TestMappingContext));
        }

        [Test, Ignore]
        public void Returns_a_default_context_with_only_object_if_no_contexts_added()
        {
            var mapper = new SessionContext();

             var result = mapper.GetContextForStory(new Story("unknown type", "totally bogus", new List<IScenario>()));
            result.ImplementingTypes.Count().ShouldEqual(1);
        }
    }

    [Context]
    public class TestMappingContext
    {
    }

    [TestFixture]
    public class Scenario_context_lifetime_rules
    {
        [Context(Lifetime = ContextLifetime.Scenario)]
        public class ScenarioLifetimeTestMappingContext
        {
        }
        private StoryContext StoryContext;

        [SetUp]
        public void SetupContext()
        {
            var mapper = new SessionContext();
            mapper.AddContext<ScenarioLifetimeTestMappingContext>();
            StoryContext = mapper.GetContextForStory(new Story("", "", new IScenario[] {}));
        }

        [Test]
        public void Context_classes_are_reused_within_one_scenario()
        {
            var context = StoryContext.GetScenarioContext();

            var context1 = context.GetContext(typeof(ScenarioLifetimeTestMappingContext));
            var context2 = context.GetContext(typeof(ScenarioLifetimeTestMappingContext));

            Assert.That(context1, Is.SameAs(context2));
        }

        [Test]
        public void Context_classes_for_different_scenarios_are_different_objects()
        {
            var context1 = StoryContext.GetScenarioContext().GetContext(typeof(ScenarioLifetimeTestMappingContext));
            var context2 = StoryContext.GetScenarioContext().GetContext(typeof(ScenarioLifetimeTestMappingContext));

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
    public class Story_context_lifetime_rules
    {
        [Context(Lifetime = ContextLifetime.Story)]
        public class TestStoryLifetimeMappingContext: IDisposable
        {
            public static bool WasDisposed;

            public TestStoryLifetimeMappingContext()
            {
                WasDisposed = false;
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

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
            return Mapper.GetContextForStory(new Story("", "", new IScenario[] { }));
        }
    }

    [TestFixture]
    public class Session_context_lifetime_rules
    {
        private readonly Type TestContextType = typeof(TestSessionLifetimeMappingContext);
        private SessionContext Mapper;

        [Context(Lifetime = ContextLifetime.Session)]
        public class TestSessionLifetimeMappingContext : IDisposable
        {
            public static bool WasDisposed;

            public TestSessionLifetimeMappingContext()
            {
                WasDisposed = false;
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

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
            return Mapper.GetContextForStory(new Story("", "", new IScenario[] { }));
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