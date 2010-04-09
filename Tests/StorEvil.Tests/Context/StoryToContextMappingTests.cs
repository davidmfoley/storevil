using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.Context.StoryToContextMapper_Specs
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
           
            View.GetWithReflection("TestMappingContext").ShouldBe(_testMappingContext);
        }
    }

    public class ContextViewer
    {
        private CodeCompiler _compiler = new CodeCompiler();

        public object Create(Dictionary<Type, object> dictionary)
        {
            var lines = dictionary.Select(x=> BuildProperty(x.Key)).ToArray();
            var propertySource = string.Join("\r\n", lines);
            var source = "public class DebugContext { \r\n" + propertySource + "\r\n}";

            var assembly = _compiler.CompileInMemory(source, new[] {  GetType().Assembly.Location});
            var context =  Activator.CreateInstance(assembly.GetTypes().First());
            foreach (var type in dictionary.Keys)
            {
                context.SetWithReflection(type.Name, dictionary[type]);
            }

            return context;
        }

        private string BuildProperty(Type type)
        {
            return string.Format("    public {0} {1} {{ get;set; }}", type.FullName, type.Name);
        }
    }

    [Context]
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

            var mapper = new StoryContextFactory();
            mapper.AddContext<TestMappingContext>();

            var context = mapper.GetContextForStory(story);
            context.ImplementingTypes.First().ShouldEqual(typeof (TestMappingContext));
        }

        [Test]
        public void Should_Register_Assembly_Types()
        {
            var mapper = new StoryContextFactory();
            mapper.AddAssembly(GetType().Assembly);

            var context = mapper.GetContextForStory(new Story("context test", "context test", new List<IScenario>()));
            context.ImplementingTypes.ShouldContain(typeof (TestMappingContext));
        }

        [Test, Ignore]
        public void Throws_if_no_context_added()
        {
            var mapper = new StoryContextFactory();

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
            var mapper = new StoryContextFactory();
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
            var mapper = new StoryContextFactory();
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