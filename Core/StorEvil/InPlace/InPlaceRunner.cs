using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceRunner : IStoryHandler
    {
        private readonly IResultListener _listener;

        private readonly ScenarioInterpreter _scenarioInterpreter;
        private readonly MemberInvoker _memberInvoker;

        private readonly IScenarioPreprocessor _preprocessor;
        private object _lastResult;

        public InPlaceRunner(IResultListener listener, 
                            IScenarioPreprocessor preprocessor,
                            ScenarioInterpreter scenarioInterpreter)
        {
            _listener = listener;
            _preprocessor = preprocessor;
            _scenarioInterpreter = scenarioInterpreter;

            _memberInvoker = new MemberInvoker();            
        }

        public int HandleStory(Story story, StoryContext context)
        {
            int failed = 0;
            _listener.StoryStarting(story);
            foreach (var scenario in GetScenarios(story))
            {
                _listener.ScenarioStarting(scenario);
                _scenarioInterpreter.NewScenario();

                using (var scenarioContext = context.GetScenarioContext())
                {
                    if (!ExecuteScenario(scenario, scenarioContext))
                        failed++;
                }
            }
            return failed;
        }

        private IEnumerable<Scenario> GetScenarios(Story story)
        {
            return story.Scenarios.SelectMany(scenario => _preprocessor.Preprocess(scenario));
        }

        public void Finished()
        {
            _listener.Finished();
        }

        private bool ExecuteScenario(Scenario scenario, ScenarioContext storyContext)
        {
            foreach (var line in scenario.Body)
            {
                if (!ExecuteLine(scenario, storyContext, line))
                    return false;
            }

            _listener.ScenarioSucceeded(scenario);
            return true;
        }

        private bool ExecuteLine(Scenario scenario, ScenarioContext storyContext, string line)
        {
            InvocationChain chain = GetMatchingChain(storyContext, line);

            if (chain == null)
            {
                ImplementationHelper _implementationHelper = new ImplementationHelper();
                var suggestion = _implementationHelper.Suggest(line);
                _listener.ScenarioPending(new ScenarioPendingInfo(scenario, line, suggestion));
                return false;
            }

            if (!ExecuteChain(scenario, storyContext, chain, line))
                return false;

            _listener.Success(scenario, line);
            return true;
        }

        private bool ExecuteChain(Scenario scenario, ScenarioContext storyContext, InvocationChain chain, string line)
        {
            string successPart = "";
            _lastResult = null;
            foreach (var invocation in chain.Invocations)
            {
                try
                {
                    InvokeContextMember(storyContext, invocation);
                    successPart += invocation.MatchedText + " ";
                }
                
                catch (TargetInvocationException ex)
                {
                    if(ex.InnerException is ScenarioPendingException) {
                        _listener.ScenarioPending(new ScenarioPendingInfo(scenario, line));
                    
                    }
                    else
                    {
                        _listener.ScenarioFailed(new ScenarioFailureInfo(scenario, successPart.Trim(), invocation.MatchedText, GetExceptionMessage(ex)));
                    }
                    
                    return false;
                }
            }

            return true;
        }

        private void InvokeContextMember(ScenarioContext storyContext, Invocation invocation)
        {
            MemberInfo info = invocation.MemberInfo;
            var declaringType = info.DeclaringType;
            var context = _lastResult ?? storyContext.GetContext(declaringType);
            _lastResult = _memberInvoker.InvokeMember(info, invocation.ParamValues, context);
        }

        private InvocationChain GetMatchingChain(ScenarioContext storyContext, string line)
        {
            var chain = _scenarioInterpreter.GetChain(storyContext, line);

            return chain;
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var ex = exception.InnerException ?? exception;

            var noStackTrace = ex is AssertionException;

            return noStackTrace ? ex.Message : ex.Message + "\r\n" + ex;
        }
    }
}