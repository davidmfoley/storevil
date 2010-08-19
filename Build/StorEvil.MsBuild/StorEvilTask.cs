using System;
using System.Collections.Generic;
using Funq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;

namespace StorEvil.MsBuild
{
    public class StorEvilTask : Task
    {
        private MsBuildTaskResultListener _resultListener;

        public override bool Execute()
        {
            var consoleMode = ParseConsoleMode();

            var settings = new ConfigSettings
                               {
                                   AssemblyLocations = Assemblies,
                                   ConsoleMode = consoleMode,
                                   StoryBasePath = StoryPath,
                                   ScenarioExtensions = StoryExtensions.Split(','),
                                   OutputFile = OutputFile,
                                   OutputFileFormat = OutputFileFormat,
                                   OutputFileTemplate = OutputFileTemplate,
                                   Debug = DebugMode
                               };

            StorEvilEvents.SetBus(new EventBus());
             _resultListener = new MsBuildTaskResultListener();
            StorEvilEvents.Bus.Register(_resultListener);
            var container = new Container();
            container.Register(settings);

            var executeSettings = new InPlaceSettings {Tags = Tags == null ? null : Tags.Split(',')};

            var configurator = new InPlaceContainerConfigurator();
            configurator.ConfigureContainer(container, settings, executeSettings);

            var job = container.Resolve<IStorEvilJob>();

            var result = job.Run() == 0;
            
            container.Dispose();

            StorEvilEvents.ResetBus();

            Passed = _resultListener.Passed.ToString();
            Pending = _resultListener.Pending.ToString();
            Failed = _resultListener.Failed.ToString();

            return result;

        }

        [Output]
        public string Passed { get; set; }

        [Output]
        public string Pending { get; set; }

        [Output]
        public string Failed { get; set; }

        private ConsoleMode ParseConsoleMode()
        {
            if (string.IsNullOrEmpty(ConsoleMode))
                return Configuration.ConsoleMode.NoColor;

            return (ConsoleMode)Enum.Parse(typeof (ConsoleMode), ConsoleMode, true);
        }

        public string Tags { get; set; }

        [Required]
        public string[] Assemblies { get; set; }

        [Required]
        public string StoryPath { get; set; }

        [Required]
        public string StoryExtensions { get; set; }

        public string OutputFile { get; set; }

        public string OutputFileFormat { get; set; }

        public string OutputFileTemplate { get; set; }

        public string ConsoleMode { get; set; }

        public bool DebugMode { get; set; }
    }

    public class MsBuildTaskResultListener : IHandle<ScenarioFinished>
    {
        public void Handle(ScenarioFinished eventToHandle)
        {
            switch (eventToHandle.Status)
            {
                case ExecutionStatus.Pending:
                case ExecutionStatus.CouldNotInterpret:
                    Pending++;
                    break;

                case ExecutionStatus.Passed:
                    Passed++;
                    break;

                case ExecutionStatus.Failed:
                    Failed++;
                    break;
            }
        }

        public int Pending { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }
    }

    
}
