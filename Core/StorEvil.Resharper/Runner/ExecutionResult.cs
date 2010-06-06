using JetBrains.ReSharper.TaskRunnerFramework;

namespace StorEvil.Resharper.Runner
{
    internal class ExecutionResult
    {
        public ExecutionResult(TaskResult status, string message)
        {
            Status = status;
            Message = message;
        }

        public TaskResult Status;
        public string Message;
    }
}