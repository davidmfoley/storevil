using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class When_scenario_does_not_map : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When scenario test does not map";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener()
        {
            ResultListener.AssertWasCalled(x => x.CouldNotInterpret(TestScenario, ScenarioText));
        }
    }

    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_succeeds : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When some action";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_success()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }

        [Test]
        public void invokes_method()
        {
            InPlaceRunnerTestContext.WhenSomeActionCalled.ShouldEqual(true);
        }
    }

    [TestFixture]
    public class mapping_by_regex_to_context_method : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "Matches a regex with 42";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_success()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }

        [Test]
        public void invokes_method_with_correct_param()
        {
            InPlaceRunnerTestContext.RegexMatchParamValue.ShouldEqual(42);
        }
    }

    [TestFixture]
    public class When_scenario_maps_to_context_method_action_that_fails : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "When some failing action";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Notifies_listener_of_failure()
        {
            ResultListener.AssertWasCalled(x => x.ScenarioFailed(Any<Scenario>(), Any<string>(), Any<string>(), Any<string>()));
        }
    }

    [TestFixture]
    public class Chaining_results : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "sub context property should be true";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_succeed()
        {
            ResultListener.AssertWasCalled(x => x.Success(TestScenario, ScenarioText));
        }
    }

    [TestFixture]
    public class Chaining_results_that_should_result_in_Failure : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private readonly Scenario TestScenario = new Scenario("test", new[] {ScenarioText});
        private const string ScenarioText = "sub context property should be false";

        [SetUp]
        public void SetupContext()
        {
            var story = new Story("test", "summary", new[] {TestScenario});

            RunStory(story);
        }

        [Test]
        public void Should_fail()
        {
            ResultListener.AssertWasCalled(x => x.ScenarioFailed(Any<Scenario>(), Any<string>(),Any<string>(),Any<string>()));
        }
    }

    public class InPlaceRunnerSpec<T>
    {
        protected IResultListener ResultListener;
        protected StoryContext Context;

        protected void RunStory(Story story)
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            Context = new StoryContext(typeof(T));
            new InPlaceRunner(ResultListener, new ScenarioPreprocessor()).HandleStory(story, Context);
        }

        protected argT Any<argT>()
        {
            return Arg<argT>.Is.Anything;
        }
    }

    [TestFixture]
    public class example_tables : InPlaceRunnerSpec<InPlaceRunnerTestContext>
    {
        private string storyText = @"
Story: test 
Scenario Outline:
Call a method with <int> and <string>
Examples:
|int|string|
|1|one|
|2|two|
|3|three|
";
        private InPlaceRunnerTableTestContext FakeContext;

        [SetUp]
        public void SetupContext()
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();

            var story = new StoryParser().Parse(storyText);
            Context = new StoryContext(typeof(InPlaceRunnerTableTestContext));
            FakeContext = MockRepository.GenerateMock<InPlaceRunnerTableTestContext>();

            new InPlaceRunner(ResultListener, new ScenarioPreprocessor()).HandleStory(story, Context);
        }
        [Test]
        public void should_invoke_method_three_times()
        {
           InPlaceRunnerTableTestContext.Calls.Count.ShouldBe(3);
        }
    }

    public class InPlaceRunnerTableTestContext
    {
        public static List<Call> Calls = new List<Call>();

        public virtual void Call_a_method_with_intParam_and_stringParam(int intParam, string stringParam)
        {
            Calls.Add(new Call(intParam, stringParam));
        }

        public class Call
        {
            public int IntParam { get; set; }
            public string StringParam { get; set; }

            public Call(int intParam, string stringParam)
            {
                IntParam = intParam;
                StringParam = stringParam;
                throw new NotImplementedException();
            }
        }
    }

    public class InPlaceRunnerTestContext
    {
        public static bool WhenSomeActionCalled;
        public static int? RegexMatchParamValue;

        public InPlaceRunnerTestContext()
        {
            WhenSomeActionCalled = false;
            RegexMatchParamValue = null;
        }

        public void WhenSomeAction()
        {
            WhenSomeActionCalled = true;
        }

        [ContextRegex("^Matches a regex with ([0-9]+)")]
        public void RegexMatchWithParam(int param)
        {
            RegexMatchParamValue = param;
        }

        public InPlaceTestSubContext SubContext()
        {
            return new InPlaceTestSubContext();
        }

        public void WhenSomeFailingAction()
        {
            throw new Exception("test exception");
        }
    }

   

    public class InPlaceTestSubContext
    {
        public object Property
        {
            get { return true; }
        }
    }
}