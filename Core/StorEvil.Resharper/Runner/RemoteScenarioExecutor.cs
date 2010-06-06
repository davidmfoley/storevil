using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.Resharper
{
    internal class RemoteScenarioExecutor
    {
        private readonly IRemoteTaskServer _server;
        private readonly ISessionContext _factory;
        private ResharperResultListener _listener;

        public RemoteScenarioExecutor(IRemoteTaskServer server, ISessionContext factory)
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

            return new StorEvilJob(provider, handler);
        }

        private InPlaceStoryRunner BuildInPlaceRunner(RemoteTask remoteTask)
        {
            _listener = new ResharperResultListener(_server, remoteTask);
            IScenarioPreprocessor preprocessor = new ScenarioPreprocessor();

            var interpreterForTypeFactory = new InterpreterForTypeFactory(new ExtensionMethodHandler());
            var resolver = new DisallowAmbiguousMatches();
            return new InPlaceStoryRunner(_listener, preprocessor, new ScenarioInterpreter(interpreterForTypeFactory, resolver), new IncludeAllFilter(), _factory );
        }
    }
}