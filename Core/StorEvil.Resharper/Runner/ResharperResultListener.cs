using System;
using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Events;
using StorEvil.Resharper.Tasks;

namespace StorEvil.Resharper.Runner
{    
    internal class ResharperResultListener : 
        IHandle<StoryStarting>, IHandle<StoryFinished>,
        IHandle<ScenarioStarting>, IHandle<ScenarioFailed>, IHandle<ScenarioPassed>, IHandle<ScenarioPending>, 
        IHandle<LineFailed>, IHandle<LinePassed>, IHandle<LinePending> 
    {
        private readonly IRemoteTaskServer _server;
        private RemoteTask _remoteTask;
        private IEnumerable<RemoteTask> _scenarioTasks = new RemoteTask[0];
        private IEnumerable<RemoteTask> _storyTasks = new RemoteTask[0];

        public TaskResult Result { get; set; }

        public ResharperResultListener(IRemoteTaskServer server)
        {
            _server = server;           
        }

        public void Handle(ScenarioPassed eventToHandle)
        {
            Result = TaskResult.Success;
            Output("... ok");
            _server.TaskFinished(_remoteTask, "ok", TaskResult.Success);

            SignalOutlineFinished();
        }

        public void Handle(ScenarioFailed eventToHandle)
        {
            Result = TaskResult.Error;
            _server.TaskFinished(_remoteTask, "failed", TaskResult.Exception);

            SignalOutlineFinished();
        }

        public void Handle(ScenarioPending eventToHandle)
        {
            Result = TaskResult.Skipped;
            _server.TaskFinished(_remoteTask, "skipped", TaskResult.Skipped);
        
            SignalOutlineFinished();
        }

        private void SignalOutlineFinished()
        {
            if (_remoteTask == null)
                return;

            var outline = FindOutline(_remoteTask);

            if (outline != null && outline.IsLastChild(((RunScenarioTask)_remoteTask).GetScenario().Id))
                _server.TaskFinished(outline, "", TaskResult.Success);
        }


        private RunScenarioOutlineTask FindOutline(RemoteTask remoteTask)
        {
            if (!(remoteTask is RunScenarioTask))
                return null;

            var id = ((RunScenarioTask) remoteTask).GetScenario().Id;

            foreach (var scenarioTask in _scenarioTasks)
            {
                if (scenarioTask is RunScenarioOutlineTask)                    
                    if (((RunScenarioOutlineTask)scenarioTask).HasChildWithId(id))
                        return scenarioTask as RunScenarioOutlineTask;
            }
            return null;
        }


        private void Output(string message)
        {
            _server.TaskOutput(_remoteTask, message + "\r\n", TaskOutputType.STDOUT);
        }

        public void SetCurrentTask(RemoteTask remoteTask)
        {
            _remoteTask = remoteTask;
        }

        public void Handle(ScenarioStarting eventToHandle)
        {
            var id = eventToHandle.Scenario.Id;

            _remoteTask = FindScenarioTask(id);
            _server.TaskStarting(_remoteTask);

            if (_remoteTask == null)
                return;

            var outline = FindOutline(_remoteTask);

            if (outline != null && outline.IsFirstChild(((RunScenarioTask)_remoteTask).GetScenario().Id))
                _server.TaskStarting(outline);

        }

        public void SetScenarioTasks(IEnumerable<RemoteTask> storyTasks, IEnumerable<RemoteTask> scenarioTasks)
        {
            _storyTasks = storyTasks;
            _scenarioTasks = scenarioTasks;
        }
        private RemoteTask FindScenarioTask(string id)
        {

            foreach (var task in _scenarioTasks)
            {
                if ( task is RunScenarioTask && ((RunScenarioTask) task).GetScenario().Id == id)
                    return task;
            }

            return null;

        }

        public void Handle(StoryStarting eventToHandle)
        {
            
            _server.TaskStarting(FindStoryTask(eventToHandle.Story.Id));
        }

        private RemoteTask FindStoryTask(string id)
        {
            foreach (var task in _storyTasks)
            {
                if (((RunStoryTask)task).Id == id)
                    return task;
            }

            return null;
        }

        public void Handle(StoryFinished eventToHandle)
        {
            _server.TaskFinished(FindStoryTask(eventToHandle.Story.Id), "", TaskResult.Success);
        }

        public void Handle(LineFailed eventToHandle)
        {
            Output(eventToHandle.SuccessPart + " [" + eventToHandle.FailedPart + "] -- failed");
            Output("----------");
            Output(eventToHandle.ExceptionInfo);

            _server.TaskException(_remoteTask,
                                  new[] {new TaskException("StorEvil failure", eventToHandle.ExceptionInfo, ""),});
            Result = TaskResult.Exception;
        }

        public void Handle(LinePassed eventToHandle)
        {
              Output(eventToHandle.Line);
        }

        public void Handle(LinePending eventToHandle)
        {
             Output("Could not interpret:\r\n" + eventToHandle.Line);               

                Output("You could try the following:");
                Output(eventToHandle.Suggestion ?? "");
        }
    }
}