using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    public class StorEvilTaskRunner : RecursiveRemoteTaskRunner
    {
        public StorEvilTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            _result = TaskResult.Success;
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            return TaskResult.Success;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            if (node.RemoteTask is RunScenarioTask)
            {
                var result = ExecuteScenario(node);
                if (result.Status != TaskResult.Success)
                    _result = result.Status;
                return result.Status;
            }
           

            return TaskResult.Success;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            return _result;
        }

        private TaskResult _result;
        private ScenarioExecutor _executor;

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            try
            {
                _result = ExecuteRecursiveInternal(node).Status;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        private ExecutionResult ExecuteRecursiveInternal(TaskExecutionNode node)
        {
            Server.TaskStarting(node.RemoteTask);
            ExecutionResult result;
            try
            {
                if (node.RemoteTask is RunProjectTask)
                    SetUpScenarioExecutorForCurrentProject(node);

                if (node.RemoteTask is RunScenarioTask)
                    result = ExecuteScenario(node);
                else
                    result = ExecuteChildTasks(node);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Server.TaskOutput(node.RemoteTask, ex + "\r\n", TaskOutputType.STDOUT);
                result = new ExecutionResult(TaskResult.Error, ex.ToString());
            }
            if (!(node.RemoteTask is RunProjectTask))
                Server.TaskFinished(node.RemoteTask, result.Message, result.Status);

            return result;
        }

        private void SetUpScenarioExecutorForCurrentProject(TaskExecutionNode node)
        {
            var projectTask = node.RemoteTask as RunProjectTask;
            var mapper = new StoryContextFactory();
            projectTask.Assemblies.ForEach(mapper.AddAssembly);

            _executor = new ScenarioExecutor(Server, mapper);
        }

        private ExecutionResult ExecuteChildTasks(TaskExecutionNode node)
        {
            var result = TaskResult.Success;

            if (node.Children != null)
            {
                foreach (TaskExecutionNode childNode in node.Children)
                {
                    var childResult = ExecuteRecursiveInternal(childNode);

                    if (childResult.Status != TaskResult.Success)
                        result = childResult.Status;
                }
            }
            
            return new ExecutionResult(result, "");
        }

        private ExecutionResult ExecuteScenario(TaskExecutionNode scenarioNode)
        {
            var remoteTask = scenarioNode.RemoteTask;
            var scenarioOrOutline = ((RunScenarioTask) remoteTask).GetScenario();
            var scenario = scenarioOrOutline as Scenario;

            if (null != scenario)
            {
                return new ExecutionResult(_executor.Execute(scenarioNode.RemoteTask), "");
            }
            return new ExecutionResult(TaskResult.Success, "");
        }
    }
}