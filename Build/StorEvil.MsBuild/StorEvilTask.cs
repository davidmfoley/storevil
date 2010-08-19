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

            var container = new Container();
            container.Register(settings);

            var executeSettings = new InPlaceSettings {Tags = Tags == null ? null : Tags.Split(',')};

            var configurator = new InPlaceContainerConfigurator();
            configurator.ConfigureContainer(container, settings, executeSettings);

          
            var job = container.Resolve<IStorEvilJob>();

            return job.Run() == 0;
        }

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
}
