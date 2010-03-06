using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.Resharper
{
    internal class ScenarioExecutor
    {
        private readonly IRemoteTaskServer _server;
        private readonly ConfigSettings _settings;
        private readonly IStoryToContextMapper _mapper;
        private ResharperResultListener _listener;

        public ScenarioExecutor(IRemoteTaskServer server, IStoryToContextMapper mapper)
        {
            _server = server;
            _mapper = mapper;
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
            InPlaceRunner handler = BuildInPlaceRunner(remoteTask);
            IStoryProvider provider = new SingleScenarioStoryProvider(scenario);

            return new StorEvilJob(provider, _mapper, handler);
        }

        private InPlaceRunner BuildInPlaceRunner(RemoteTask remoteTask)
        {
            _listener = new ResharperResultListener(_server, remoteTask);
            IScenarioPreprocessor preprocessor = new ScenarioPreprocessor();

            return new InPlaceRunner(_listener, preprocessor);
        }
    }
}