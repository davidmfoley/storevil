using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.Utility;

namespace StorEvil.Context.Lifetime
{
    [TestFixture]
    public class Dependent_contexts_with_conflicting_lifetimes
    {
        [Test]
        public void Throws_an_exception_with_a_message_indicating_the_problem()
        {
            var sessionContext = new SessionContext();
            sessionContext.AddContext<ScenarioLifetimeTestMappingContext>();
            sessionContext.AddContext<SessionLifetimeDependingOnScenarioLifetime>();
            var scenarioContext = sessionContext.GetContextForStory().GetScenarioContext();

            var ex = Expect.ThisToThrow<ConflictingLifetimeException>(()=>scenarioContext.GetContext(typeof(SessionLifetimeDependingOnScenarioLifetime)));
            ex.Message.ShouldContain(typeof(SessionLifetimeDependingOnScenarioLifetime).ToString());
            ex.Message.ShouldContain(typeof(ScenarioLifetimeTestMappingContext).ToString());
        }
    }

    [Context(Lifetime = ContextLifetime.Session)]
    public class SessionLifetimeDependingOnScenarioLifetime
    {
        public SessionLifetimeDependingOnScenarioLifetime(ScenarioLifetimeTestMappingContext s)
        {
        }
    }

    [TestFixture]
    public class Scenario_context_lifetime_rules
    {
       
        private StoryContext StoryContext;

        [SetUp]
        public void SetupContext()
        {
            var mapper = new SessionContext();
            mapper.AddContext<ScenarioLifetimeTestMappingContext>();
            StoryContext = mapper.GetContextForStory();
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
}