using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.Resharper
{
    internal class ScenarioExecutor
    {
        private readonly IRemoteTaskServer _server;
        private readonly IStoryContextFactory _factory;
        private ResharperResultListener _listener;

        public ScenarioExecutor(IRemoteTaskServer server, IStoryContextFactory factory)
        {
            _server = server;
            _factory = factory;
        }

        public TaskResult Execute(RemoteTask remoteTask)
        {
            try
            {
                var scenario = ((RunScenarioTask) remoteTask).GetScenario();
                var job = GetJob(remoteTask, scenario);

                job.Run();

                return _listener.Result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                _server.TaskOutput(remoteTask, "Exception!\r\n", TaskOutputType.STDOUT);
                _server.TaskOutput(remoteTask, ex.ToString(), TaskOutputType.STDOUT);

                return TaskResult.Exception;
            }
        }

        private IStorEvilJob GetJob(RemoteTask remoteTask, IScenario scenario)
        {
            
            InPlaceStoryRunner handler = BuildInPlaceRunner(remoteTask);
            IStoryProvider provider = new SingleScenarioStoryProvider(scenario);

            return new StorEvilJob(provider, _factory, handler);
        }

        private InPlaceStoryRunner BuildInPlaceRunner(RemoteTask remoteTask)
        {
            _listener = new ResharperResultListener(_server, remoteTask);
            IScenarioPreprocessor preprocessor = new ScenarioPreprocessor();

            return new InPlaceStoryRunner(_listener, preprocessor, new ScenarioInterpreter(new InterpreterForTypeFactory(new ExtensionMethodHandler())), new IncludeAllFilter(), _factory );
        }
    }
}