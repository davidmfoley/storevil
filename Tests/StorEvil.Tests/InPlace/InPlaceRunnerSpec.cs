using System;
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
        protected InPlaceRunner Runner;

        protected void RunStory(Story story)
        {
            ResultListener = MockRepository.GenerateStub<IResultListener>();
            new ExtensionMethodHandler().AddAssembly(typeof(TestExtensionMethods).Assembly);

            Context = new StoryContext(typeof (T));
            Runner = new InPlaceRunner(ResultListener, new ScenarioPreprocessor(), new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler())));
            Runner.HandleStory(story, Context);
        }

        protected argT Any<argT>()
        {
            return Arg<argT>.Is.Anything;
        }
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