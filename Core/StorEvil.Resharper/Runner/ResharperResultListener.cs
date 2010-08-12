using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Events;

namespace StorEvil.Resharper.Runner
{    
    internal class ResharperResultListener : IHandle<ScenarioFinished>, IHandle<LineExecuted>
    {
        private readonly IRemoteTaskServer _server;
        private RemoteTask _remoteTask;

        public TaskResult Result { get; set; }

        public ResharperResultListener(IRemoteTaskServer server)
        {
            _server = server;           
        }

        public void Handle(ScenarioFinished eventToHandle)
        {
            if (eventToHandle.Status == ExecutionStatus.Passed)
            {
                Result = TaskResult.Success;
                Output("... ok");
                
            }          
            else if (eventToHandle.Status == ExecutionStatus.Failed)
            {
                Result = TaskResult.Error;
            }
            else
            {
                Result = TaskResult.Skipped;
            }
        }

        public void Handle(LineExecuted eventToHandle)
        {
            var line = eventToHandle.Line;

            if (eventToHandle.Status == ExecutionStatus.Pending)
            {
                Output("Could not interpret:\r\n" + line);               

                Output("You could try the following:");
                Output(eventToHandle.Suggestion ?? "");
            
                return;
            }

            if (eventToHandle.Status == ExecutionStatus.Failed)
            {
                Output(eventToHandle.SuccessPart + " [" + eventToHandle.FailedPart + "] -- failed");
                Output("----------");
                Output(eventToHandle.Message);

                _server.TaskException(_remoteTask, new[] { new TaskException("StorEvil failure", eventToHandle.Message, ""), });
                Result = TaskResult.Exception;
                return;
            }

            Output(line);
        }

        private void Output(string message)
        {
            _server.TaskOutput(_remoteTask, message + "\r\n", TaskOutputType.STDOUT);
        }

        public void SetCurrentTask(RemoteTask remoteTask)
        {
            _remoteTask = remoteTask;
        }
    }
}