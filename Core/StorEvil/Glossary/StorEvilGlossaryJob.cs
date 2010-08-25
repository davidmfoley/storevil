using System.Linq;
using StorEvil.Core;
using StorEvil.Events;

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
}
