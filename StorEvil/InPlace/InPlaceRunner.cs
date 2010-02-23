using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public class InPlaceRunner : IStoryHandler
    {
        private readonly IResultListener _listener;

        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly MemberInvoker _memberInvoker;

        private readonly IScenarioPreprocessor _preprocessor;

        public InPlaceRunner(IResultListener listener, IScenarioPreprocessor preprocessor)
        {
            _listener = listener;
            _preprocessor = preprocessor;
            _scenarioInterpreter = new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler()));
            _memberInvoker = new MemberInvoker();
        }

        public void HandleStory(Story story, StoryContext context)
        {
            _listener.StoryStarting(story);
            foreach (var scenario in GetScenarios(story))
            {
                _listener.ScenarioStarting(scenario);

                ExecuteScenario(scenario, context.GetScenarioContext());
            }
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            foreach (var scenario in story.Scenarios)
            {
                foreach (var s in _preprocessor.Preprocess(scenario))
                {
                    yield return s;
                }
            }
        }

        public void Finished()
        {
            
        }

        private void ExecuteScenario(Scenario scenario, ScenarioContext storyContext)
        {
            string lastSignificantFirstWord = null;
            foreach (var line in scenario.Body)
            {

                var chain = _scenarioInterpreter.GetChain(storyContext, line);

                if (chain == null)
                {
                    if ((line.FirstWord().ToLower() == "and"))
                        chain = _scenarioInterpreter.GetChain(storyContext, line.ReplaceFirstWord(lastSignificantFirstWord));

                    if (chain == null)
                    {
                        _listener.CouldNotInterpret(scenario, line);
                        return;
                    }
                }

                if (!(line.FirstWord().ToLower() == "and"))
                    lastSignificantFirstWord = line.FirstWord();

                object lastResult = null;
                string successPart = "";
                foreach (var invocation in chain.Invocations)
                {
                    MemberInfo info = invocation.MemberInfo;
                    var declaringType = info.DeclaringType;

                    var context = lastResult ?? storyContext.GetContext(declaringType);

                    try
                    {
                        lastResult = _memberInvoker.InvokeMember(info, invocation.ParamValues, context);
                    }
                    catch (Exception ex)
                    {
                        _listener.ScenarioFailed(scenario, successPart.Trim(), invocation.MatchedText, GetExceptionMessage(ex));
                        return;
                    }
                    successPart += invocation.MatchedText + " ";
                }
                // for now
                _listener.Success(scenario, line);
            }
        }

        private string GetExceptionMessage(Exception exception)
        {
            Exception ex;

            if (exception.InnerException != null)

                ex = exception.InnerException;
            else
                ex = exception;

            var msg = ex.Message;

            if (!(ex is AssertionException))
                msg += "\r\n" + ex.ToString();  
            return msg;

        }
    }
}