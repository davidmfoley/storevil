using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Funq;
using Spark;
using StorEvil.Configuration;
using StorEvil.Context.Matchers;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.Spark;
using StorEvil.Utility;

namespace StorEvil.Glossary
{
    public class StorEvilGlossaryJob : IStorEvilJob
    {
        private readonly IStepProvider _stepProvider;
        private readonly IStepDescriber _stepDescriber;
        private readonly IEventBus _bus;
        private readonly IGlossaryFormatter _formatter;

        public StorEvilGlossaryJob(IStepProvider stepProvider, IStepDescriber stepDescriber, IEventBus bus, IGlossaryFormatter formatter)
        {
            _stepProvider = stepProvider;
            _stepDescriber = stepDescriber;
            _bus = bus;
            _formatter = formatter;
        }

        public int Run()
        {
            var stepDefinitions = _stepProvider
                .GetSteps();
            var descriptions = stepDefinitions
                .Select(x => _stepDescriber.Describe(x));

            foreach (var stepDescription in descriptions.OrderBy(x => x.Description))
                _bus.Raise(new GenericInformation { Text = stepDescription.Description });

            var glossary = new Glossary {Steps = stepDefinitions};
            _formatter.Handle(glossary) ;
            return 0;
        }
    }

    public interface IStepDescriber
    {
        StepDescription Describe(StepDefinition stepDefinition);
    }

    public class StepDefinition
    {
        public StepDefinition()
        {
            Children = new StepDefinition[0];
        }

        public Type DeclaringType { get; set; }

        public IMemberMatcher Matcher { get; set; }

        public IEnumerable<StepDefinition> Children { get; set; }
    }


    public class GlossaryConfigurator : ContainerConfigurator<GlossaryConfigurator.GlossarySettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, GlossarySettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilGlossaryJob>();
            container.EasyRegister<IStepDescriber, StepDescriber>();
            container.EasyRegister<IStepProvider, StepProvider>();
            container.EasyRegister<ContextTypeFactory>();

            var templateSpecifiied = !(string.IsNullOrEmpty(customSettings.GlossaryTemplate) );

            DebugTrace.Trace(this, "GlossaryTemplate:" + customSettings.GlossaryTemplate);
            DebugTrace.Trace(this, "GlossaryDestination:" + customSettings.GlossaryDestination);
            if (!templateSpecifiied)
            {
                container.Register<IGlossaryFormatter>(new NoOpGlossaryFormatter());
            }
            else
            {

                var glossaryDestination = string.IsNullOrEmpty(customSettings.GlossaryDestination) ? "StorEvilGlossary.html" : customSettings.GlossaryDestination;
                DebugTrace.Trace(this, "GlossaryDestination:" + glossaryDestination);
                container.Register<IGlossaryFormatter>(new SparkGlossaryFormatter(new FileWriter(glossaryDestination, true), customSettings.GlossaryTemplate));
            }
        }

        public class GlossarySettings
        {
            public string GlossaryDestination { get; set; }
            public string GlossaryTemplate { get; set; }
        }
    }

    public class NoOpGlossaryFormatter : IGlossaryFormatter
    {
        public void Handle(Glossary glossary)
        {

        }
    }

    public interface IGlossaryFormatter
    {
        void Handle(Glossary glossary);
    }

    public class Glossary
    {
        public IEnumerable<StepDefinition> Steps { get; set; }
    }

    public class SparkGlossaryFormatter : IGlossaryFormatter
    {
        private readonly ITextWriter _fileWriter;
        private readonly string _pathToTemplateFile;

        public SparkGlossaryFormatter(ITextWriter fileWriter, string pathToTemplateFile)
        {
            _fileWriter = fileWriter;
            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(Glossary glossary)
        {
            var generator = new SparkReportGenerator<GlossaryView>(_fileWriter, _pathToTemplateFile);
            generator.Handle(GetViewModel(glossary));
        }

        private GlossaryViewModel GetViewModel(Glossary glossary)
        {
            var transformed = glossary.Steps.Select(x => BuildViewModelStep(x));
            return new GlossaryViewModel {Steps = transformed};
        }

        private StepViewModel BuildViewModelStep(StepDefinition stepDefinition)
        {
            var describer = new StepDescriber();

            return new StepViewModel
                       {
                           TypeName = stepDefinition.DeclaringType.FullName,
                           MemberName = stepDefinition.Matcher.MemberInfo.Name,
                           Description = describer.Describe(stepDefinition),
                           IsExtensionMethod = IsExtensionMethod(stepDefinition.Matcher.MemberInfo),
                           Children = stepDefinition.Children.Select(BuildViewModelStep)
                       };
        }

        private bool IsExtensionMethod(MemberInfo memberInfo)
        {
            return memberInfo.IsDefined(typeof (ExtensionAttribute), true);          
        }
    }

    public class StepViewModel
    {
        public StepDescription Description;
        public bool IsExtensionMethod;

        public string TypeName;

        public string MemberName;

        public IEnumerable<StepViewModel> Children { get; set; }
    }


    public abstract class GlossaryView : AbstractSparkView
    {
        public GlossaryViewModel Model { get; set; }
        public string HTML(object h)
        {
            DebugTrace.Trace(this, "HTML:" + h);
            return h.ToString();
        }

        public string FormatDescription(StepDescription description)
        {
            // HACK
            return description.Description
                .Replace("<", "<span class=\"param\">")
                .Replace(">", "</span>");
        }
    }

    public class GlossaryViewModel
    {
        public IEnumerable<StepViewModel> Steps { get; set; }
    }
}
