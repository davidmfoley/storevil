using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.Resharper
{
    internal class ResharperResultListener : IResultListener
    {
        private readonly IRemoteTaskServer _server;
        private readonly RemoteTask _remoteTask;

        public TaskResult Result { get; set; }

        public ResharperResultListener(IRemoteTaskServer server, RemoteTask remoteTask)
        {
            _server = server;
            _remoteTask = remoteTask;
        }

        public void StoryStarting(Story story)
        {
        }

        public void ScenarioStarting(Scenario scenario)
        {
            Result = TaskResult.Success;
        }

        public void ScenarioFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            Output(successPart + " [" + failedPart + "] -- failed");
            Output("----------");
            Output(message);

            _server.TaskException(_remoteTask, new[] {new TaskException("StorEvil failure", message, ""),});
            Result = TaskResult.Exception;
        }

        private void Output(string message)
        {
            _server.TaskOutput(_remoteTask, message + "\r\n", TaskOutputType.STDOUT);
        }

        public void CouldNotInterpret(Scenario scenario, string line)
        {
            Output("Could not interpret:\r\n" + line);
            var suggestion = new ImplementationHelper().Suggest(line);

            Output("You could try the following:");
            Output(suggestion);

            Result = TaskResult.Skipped;
        }

        public void Success(Scenario scenario, string line)
        {
            Output(line);
        }

        public void ScenarioSucceeded(Scenario scenario)
        {
            Output("ok");
        }

        public void Finished()
        {
            
        }
    }
}