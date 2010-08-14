using System;
using System.Collections.Generic;
using System.Reflection;
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
        private readonly List<string> _loadedAssemblies = new List<string>();
        private readonly AssemblyLoader _loader = new AssemblyLoader();
        private string _logIndent = " ";

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
            Logger.Log(_logIndent+ "ExecuteRecursiveInternal: " + node.RemoteTask.GetType());
            _logIndent += " ";
            ExecutionResult result;
            try
            {
                if (node.RemoteTask is RunProjectTask)
                    SetUpScenarioExecutorForCurrentProject(node);

                if (node.RemoteTask is LoadContextAssemblyTask)
                    ExecuteLoadContextAssemblyTask(node);

                if (node.RemoteTask is RunScenarioTask)
                    result = ExecuteScenario(node);
                else
                    result = ExecuteChildTasks(node);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                Server.TaskOutput(node.RemoteTask, ex + "\r\n", TaskOutputType.STDOUT);
                Server.TaskError(node.RemoteTask, ex.ToString());
                result = new ExecutionResult(TaskResult.Error, ex.ToString());
            }

            if (result != null)
                Server.TaskFinished(node.RemoteTask, result.Message, result.Status);
            else
                Server.TaskFinished(node.RemoteTask, "", TaskResult.Success);

            _logIndent = _logIndent.Substring(0, _logIndent.Length - 1);

            Logger.Log(_logIndent + "ExecuteRecursiveInternal finished: " + node.RemoteTask.GetType()+ " result:" + (result != null ? result.Status.ToString() : "unknown"));

            return result;
        }

        private void ExecuteLoadContextAssemblyTask(TaskExecutionNode node)
        {
            var lcat = node.RemoteTask as LoadContextAssemblyTask;

            var path = lcat.AssemblyPath;
            if (!_loadedAssemblies.Contains(path))
            {               
                _loadedAssemblies.Add(path);
                _loader.RegisterAssembly(Assembly.LoadFrom(path));
            }
        }

        private void SetUpScenarioExecutorForCurrentProject(TaskExecutionNode node)
        {
            var projectTask = node.RemoteTask as RunProjectTask;

            var assemblyRegistry = new AssemblyRegistry(projectTask.Assemblies);

            _executor = new RemoteScenarioExecutor(Server, assemblyRegistry);
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
                    {
                        if (result == TaskResult.Skipped || result == TaskResult.Success)
                            result = childResult.Status;
                    }
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