using System;
using System.Linq;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class InPlaceRunnerSpec<T>
    {
        protected IResultListener ResultListener;
        protected StoryContext Context;
       

        protected void RunStory(Story story)
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();
            new ExtensionMethodHandler().AddAssembly(typeof(TestExtensionMethods).Assembly);

            Context = new StoryContext(typeof (T));

            if (UseCompilingRunner())
            {
                var runner = new InPlaceCompilingStoryRunner(ResultListener, new ScenarioPreprocessor(),
                                                new ScenarioInterpreter(
                                                    new InterpreterForTypeFactory(new ExtensionMethodHandler())),
                                                new IncludeAllFilter());
                runner.HandleStory(story, Context);
            }
            else
            {
                var runner = new InPlaceStoryRunner(ResultListener, new ScenarioPreprocessor(),
                                                new ScenarioInterpreter(
                                                    new InterpreterForTypeFactory(new ExtensionMethodHandler())),
                                                new IncludeAllFilter());
                runner.HandleStory(story, Context);
            }

        }

        private bool UseCompilingRunner()
        {
            return (null != this as UsingCompiledRunner);
        }

        protected argT Any<argT>()
        {
            return Arg<argT>.Is.Anything;
        }

        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            int lineNumber = 0;
            return new Scenario("test", lines.Select(line=> new ScenarioLine {Text = line, LineNumber = ++lineNumber}).ToArray());
        }
    }

    internal interface UsingCompiledRunner
    {
    }

    public class InPlaceRunnerTestContext
    {
        public static bool WhenSomeActionCalled;
        public static int? RegexMatchParamValue;
        public static bool WhenSomeOtherActionCalled;
        public static string MultiWordParam;

        public InPlaceRunnerTestContext()
        {
            WhenSomeActionCalled = false;
            RegexMatchParamValue = null;
            MultiWordParam = null;
        }

        public void WhenSomeAction()
        {
            WhenSomeActionCalled = true;
        }

        public void WhenSomeOtherAction()
        {
            WhenSomeOtherActionCalled = true;
        }

        [ContextRegex("^Matches a regex with ([0-9]+)")]
        public void RegexMatchWithParam(int param)
        {
            RegexMatchParamValue = param;
        }

        public void Pending_scenario_step()
        {
           ScenarioStatus.Pending();
        }

        public void test_param_with_multiple_words([MultipleWords] string param)
        {
            MultiWordParam = param;
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