using JetBrains.ReSharper.TaskRunnerFramework;

namespace StorEvil.Resharper
{
    public class StorEvilTaskRunner : RecursiveRemoteTaskRunner
    {
        public StorEvilTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        // Called to prepare this task. Shouldn't throw
        // Server.TaskStarting is called before this
        // If Start returns TaskError or TaskException, Server.TaskFinished is called
        // If Start returns TaskSkipped, all child nodes have server.TaskFinished(TaskResult.Skipped)
        // called recursively. It is up to Start to call TaskFinished(Skipped) for the current node 
        public override TaskResult Start(TaskExecutionNode node)
        {
            return TaskResult.Skipped;
        }

        // Called to run the task, unless we implement RecursiveRemoteTaskRunner, in which case it won't
        // Can throw. Exception will be caught and logged with Server.TaskException
        // Finish will be called, and then Server.TaskFinished - but with TaskResult.Exception
        // Return TaskSuccess to get the child nodes executed
        public override TaskResult Execute(TaskExecutionNode node)
        {
            return TaskResult.Skipped;
        }

        // Called for cleanup purposes. For a successful run, the return value of this is
        // the return value of the task. If an exception is thrown, the return value is
        // ignored and TaskException is returned. If the node is skipped, this doesn't get
        // called.
        // Should not throw
        public override TaskResult Finish(TaskExecutionNode node)
        {
            return TaskResult.Skipped;
        }

        // Called to handle all the nodes ourselves
        public override void ExecuteRecursive(TaskExecutionNode node)
        {
        }

        public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
        {
        }
    }
}