using System;
using System.Collections.Generic;
using Funq;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public class InPlaceContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStoryHandler, InPlaceStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryFilter>(new TagFilter(CustomSettings.Tags ?? new string[0]));
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }

    public class InPlaceDebugContainerConfigurator : ContainerConfigurator<InPlaceSettings>
    {
        protected override void SetupCustomComponents(Container container)
        {
            container.EasyRegister<IStoryHandler, InPlaceCompilingStoryRunner>();
            container.EasyRegister<IStorEvilJob, StorEvilJob>();
            container.Register<IStoryFilter>(new TagFilter(CustomSettings.Tags ?? new string[0]));
        }

        protected override void SetupSwitches(SwitchParser<InPlaceSettings> parser)
        {
            parser.AddSwitch("--tags", "-g")
                .SetsField(x => x.Tags);
        }
    }
}