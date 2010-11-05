using System;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class executing_scenario_with_background
        : InPlace.executing_scenario_with_background, UsingCompiledRunner
    {
    }
}


namespace StorEvil.InPlace.NonCompiled
{
    [TestFixture]
    public class executing_scenario_with_background
        : InPlace.executing_scenario_with_background, UsingNonCompiledRunner
    {
    }
}
namespace StorEvil.InPlace
{
    public abstract class executing_scenario_with_background :
        InPlaceRunnerSpec<executing_scenario_with_background.ScenarioBackgroundTestContext>
    {
        private string storyText =
            @"
Story: test backgrounds in scenarios

Background:
Run the background

Scenario:
Background run count should be 1

Scenario:
Background run count should be 2

Scenario:
Background run count should be 3
";

        [SetUp]
        public void SetupContext()
        {
            ScenarioBackgroundTestContext.Reset();
            RunStory(new StoryParser().Parse(storyText, null));
        }

        [Test]
        public void runs_background()
        {
            AssertLineSuccess("Run the background", 3);
        }

        [Test]
        public void is_successful()
        {
            AssertLineSuccess("Background run count should be 1");
            AssertLineSuccess("Background run count should be 2");
            AssertLineSuccess("Background run count should be 3");
        }

        [Context]
        public class ScenarioBackgroundTestContext
        {
            private static int _runCount;

            public void Run_The_Background()
            {
                _runCount++;
            }

            public void Background_Run_Count_Should_Be(int expected)
            {
                _runCount.ShouldEqual(expected);
            }

            public static void Reset()
            {
                _runCount = 0;
            }
        }
    }
}