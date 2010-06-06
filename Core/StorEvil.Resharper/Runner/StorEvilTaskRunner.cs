using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Resharper.Tasks;

namespace StorEvil.Resharper.Runner
{
    public class StorEvilTaskRunner : RecursiveRemoteTaskRunner
    {
        public StorEvilTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            Logger.Log("StorEvilTaskRunner constructed");
            server.ClientMessage("TaskRunner starting");
            
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
        private RemoteScenarioExecutor _executor;
        public static string RunnerId = "StorEvilRunner";

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            try
            {
                Logger.Log("ExecuteRecursive");
                Logger.Log(node.RemoteTask.RunnerID);
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
            var mapper = new SessionContext();
            projectTask.Assemblies.ForEach(mapper.AddAssembly);

            _executor = new RemoteScenarioExecutor(Server, mapper);
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