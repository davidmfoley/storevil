using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.Interpreter.ParameterConverters;
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
        private IStoryHandler _runner;
        private MostRecentlyUsedContext _resolver;

        public RemoteScenarioExecutor(IRemoteTaskServer server, AssemblyRegistry assemblyRegistry)
        {
            _server = server;
            _assemblyRegistry = assemblyRegistry;
            _sessionContext = new SessionContext(assemblyRegistry);

            _eventBus = new EventBus();

            new EventBusAutoRegistrar(_assemblyRegistry).InstallTo(_eventBus);
            _listener = new ResharperResultListener(_server);
          
            _resolver = new MostRecentlyUsedContext();
            _runner = BuildInPlaceRunner(_resolver);

            _eventBus.Register(_resolver);
            _eventBus.Register(_listener);

            ParameterConverter.AddCustomConverters(_assemblyRegistry);
        }

        public ExecutionResult Execute(IEnumerable<TaskExecutionNode> storyNodes)
        {
            var stories = new List<Story>();

            var scenarioTasks = new List<RemoteTask>();
            var storyTasks = new List<RemoteTask>();
          
            foreach (var storyNode in storyNodes)
            {
                var rst = storyNode.RemoteTask as RunStoryTask;
                storyTasks.Add(rst);
                _server.TaskStarting(storyNode.RemoteTask);

                var scenarios = new List<IScenario>();
                foreach (var task in storyNode.Children)
                {
                    if (task.RemoteTask is RunScenarioTask)
                        scenarios.Add(GetScenarioFromNode(task));  
                    else
                    {
                        foreach (var node in (task.Children))
                        {
                            scenarios.Add(GetScenarioFromNode(node));
                            scenarioTasks.Add(node.RemoteTask);
                        }
                    }
                    scenarioTasks.Add(task.RemoteTask);
                }

                stories.Add(new Story(rst.Id, rst.Id, scenarios));
            }

            _listener.SetScenarioTasks(storyTasks, scenarioTasks);
            _runner.HandleStories(stories);

            return new ExecutionResult(TaskResult.Success, "");
        }

        private IScenario GetScenarioFromNode(TaskExecutionNode task)
        {
            return ((RunScenarioTask)task.RemoteTask).GetScenario();
        }

        private IEnumerable<RemoteTask> GetScenarioChildTasks(List<TaskExecutionNode> children)
        {
            foreach (var node in children)
            {
                yield return node.RemoteTask;

                if (node.RemoteTask is RunScenarioOutlineTask)
                {
                   
                    foreach (var taskExecutionNode in node.Children)
                    {
                        yield return taskExecutionNode.RemoteTask as RunScenarioTask;
                    }
                }                
            }
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

        private IStoryHandler BuildInPlaceRunner(IAmbiguousMatchResolver resolver)
        {
            var sameDomainHandlerFactory = new SameDomainHandlerFactory(new AssemblyGenerator(), _assemblyRegistry,new Filesystem());
            return new InPlaceCompilingStoryRunner(sameDomainHandlerFactory, new IncludeAllFilter(),  _eventBus);
        }

    }
}