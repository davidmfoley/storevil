using System.Collections.Generic;
using Funq;
using StorEvil.Configuration;
using StorEvil.Utility;

namespace StorEvil.Core
{
    public class StorEvilGlossaryJob : IStorEvilJob
    {
        private readonly IStepProvider _stepProvider;
        private readonly IStepDescriber _stepDescriber;

        public StorEvilGlossaryJob(IStepProvider stepProvider, IStepDescriber stepDescriber)
        {
            _stepProvider = stepProvider;
            _stepDescriber = stepDescriber;
        }

        public int Run()
        {
            foreach (var stepDescription in _stepProvider.GetSteps())
            {
                _stepDescriber.Describe(stepDescription);
            }
            return 0;
        }
    }


    public interface IStepDescriber
    {
        void Describe(StepDefinition stepDefinition);
    }
    public interface IStepProvider
    {
        IEnumerable<StepDefinition> GetSteps();
    }

    public class StepDefinition
    {
    }


    public class GlossaryConfigurator : ContainerConfigurator<GlossarySettings>
    {
        protected override void SetupCustomComponents(Container container, ConfigSettings configSettings, GlossarySettings customSettings)
        {
            container.EasyRegister<IStorEvilJob, StorEvilGlossaryJob>();
        }
    }

    public class GlossarySettings
    {
    }
}