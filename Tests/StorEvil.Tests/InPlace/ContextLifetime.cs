
using System;
using NUnit.Framework;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public abstract class Disposing_contexts_with_default_lifetime : InPlaceRunnerSpec<InPlaceRunnerDisposalTestContext>
    {
        private readonly Scenario TestScenario1 = BuildScenario("test", "when a disposable context is used");
        private readonly Scenario TestScenario2 = BuildScenario("test", "then it should be disposed");

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] { TestScenario1, TestScenario2 });

            RunStory(story);
        }

        [Test]
        public void Should_succeed()
        {
            AssertLineSuccess("then it should be disposed");

        }
    }



    public abstract class Disposing_contexts_with_story_lifetime : InPlaceRunnerSpec<StoryLifetimeDisposalTestContext>
    {
        private readonly Scenario TestScenario1 = BuildScenario("test1", "when a disposable context with a story lifetime is used", "story lifetime dispose calls should be 0");
        private readonly Scenario TestScenario2 = BuildScenario("test2", "when a disposable context with a story lifetime is used", "story lifetime dispose calls should be 0");
        private readonly Scenario TestScenario3 = BuildScenario("test3", "when a disposable context with a story lifetime is used", "story lifetime dispose calls should be 0");

        [SetUp]
        public void SetupContext()
        {
            StoryLifetimeDisposalTestContext.DisposeCalls = 0;

            var story = new Story("test", "summary", new[] { TestScenario1, TestScenario2, TestScenario3 });

            RunStory(story);
        }

        [Test]
        public void First_time_should_not_have_disposed()
        {
            string name = "test1";
            AssertScenarioSuccessWithName(name);
        }

        [Test]
        public void Second_time_should_not_have_disposed()
        {
            string name = "test2";
            AssertScenarioSuccessWithName(name);
        }

        [Test]
        public void Third_time_should_not_have_disposed()
        {
            string name = "test3";
            AssertScenarioSuccessWithName(name);
        }

        [Test]
        public void All_scenarios_should_succeed()
        {
            AssertAllScenariosSucceeded();
        }
    }

    [Context]
    public class InPlaceRunnerDisposalTestContext : IDisposable
    {
        private static int DisposeCalls = 0;
        public void when_a_disposable_context_is_used() { }
        public void then_it_should_be_disposed()
        {
            Assert.That(DisposeCalls, Is.GreaterThan(0));
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }

    [Context(Lifetime = ContextLifetime.Story)]
    public class StoryLifetimeDisposalTestContext : IDisposable
    {
        public static int DisposeCalls = 0;
        public void when_a_disposable_context_with_a_story_lifetime_is_used() { }

        public int story_lifetime_dispose_calls()
        {
            return DisposeCalls;
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }

    [Context(Lifetime = ContextLifetime.Session)]
    public class SessionLifetimeDisposalTestContext : IDisposable
    {
        public static int DisposeCalls = 0;
        public void when_a_disposable_context_with_a_session_lifetime_is_used() { }

        public int session_lifetime_dispose_calls()
        {
            return DisposeCalls;
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }
}