using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context.WordFilters;
using StorEvil.Core;
using StorEvil.Events;

namespace StorEvil.Glossary
{
    [TestFixture]
    public class glossary_job
    {
        private StorEvilGlossaryJob Job;
        private IStepProvider _fakeStepProvider;
        private IStepDescriber _fakeStepDescriber;

        [SetUp]
        public void SetUpContext()
        {
            _fakeStepProvider = MockRepository.GenerateStub<IStepProvider>();
            _fakeStepDescriber = MockRepository.GenerateStub<IStepDescriber>();
            Job = new StorEvilGlossaryJob(_fakeStepProvider, _fakeStepDescriber, new EventBus(), new NoOpGlossaryFormatter());
        }

        [Test]
        public void returns_0()
        {
            StepProviderReturns();
            Job.Run().ShouldEqual(0);
        }

        [Test]
        public void sends_step_defs_to_formatter()
        {
            var definition1 = new StepDefinition();
            var definition2 = new StepDefinition();

            StepProviderReturns(definition1, definition2);
            _fakeStepDescriber.Stub(x => x.Describe(Arg<StepDefinition>.Is.Anything)).Return(new StepDescription());
            Job.Run();

            _fakeStepDescriber.AssertWasCalled(x => x.Describe(definition1));
            _fakeStepDescriber.AssertWasCalled(x => x.Describe(definition2));
        }

        private void StepProviderReturns(params StepDefinition[] definitions)
        {
            _fakeStepProvider
                .Stub(x => x.GetSteps())
                .Return(definitions);
        }
    }
}