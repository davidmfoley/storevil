using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Parsing;
using StorEvil.Resharper.Tasks;

namespace StorEvil.Resharper.Runner
{
    internal class RemoteScenarioExecutor
    {
        private readonly IRemoteTaskServer _server;
        private readonly AssemblyRegistry _assemblyRegistry;
        private readonly ISessionContext _sessionContext;
        private ResharperResultListener _listener;
        private EventBus _eventBus;
        private InPlaceStoryRunner _runner;
        private MostRecentlyUsedContext _resolver;

        public RemoteScenarioExecutor(IRemoteTaskServer server, AssemblyRegistry assemblyRegistry)
        {
            _server = server;
            _assemblyRegistry = assemblyRegistry;
            _sessionContext = new SessionContext(assemblyRegistry);

            _eventBus = new EventBus();
            _listener = new ResharperResultListener(_server);
          

            _resolver = new MostRecentlyUsedContext();
            _runner = BuildInPlaceRunner(_resolver);

            _eventBus.Register(_resolver);
            _eventBus.Register(_listener);
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

            _listener.SetCurrentTask(remoteTask);
            IStoryProvider provider = new SingleScenarioStoryProvider(scenario);

            return new StorEvilJob(provider, _runner);
        }

        private InPlaceStoryRunner BuildInPlaceRunner(IAmbiguousMatchResolver resolver)
        {
            IScenarioPreprocessor preprocessor = new ScenarioPreprocessor();

         
            var interpreterForTypeFactory = new InterpreterForTypeFactory(new ExtensionMethodHandler(_assemblyRegistry));

            var scenarioInterpreter = new ScenarioInterpreter(interpreterForTypeFactory, resolver);

            return new InPlaceStoryRunner(preprocessor, scenarioInterpreter, new IncludeAllFilter(), _sessionContext, _eventBus );
        }
    }
}