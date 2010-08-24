using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Assertions;
using StorEvil.Context.Matchers;
using StorEvil.Context.WordFilters;
using StorEvil.Core;
using StorEvil.Interpreter;

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
            Job = new StorEvilGlossaryJob(_fakeStepProvider, _fakeStepDescriber);
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

    [TestFixture]
    public class Step_describer
    {
        private StepDescriber Describer;

        [SetUp]
        public void SetUpContext()
        {
            Describer = new StepDescriber();
        }

        [Test]
        public void can_describe_a_reflection_step()
        {
            string result = GetMethodNameMatcherResult("This_is_an_example");

            result.ShouldEqual("This is an example");
        }

        [Test]
        public void can_describe_a_reflection_step_with_a_parameter()
        {
            string result = GetMethodNameMatcherResult("This_is_an_example_with_a_parameter");

            result.ShouldEqual("This is an example with a <int parameter>");
        }

        private string GetMethodNameMatcherResult(string methodName)
        {
            var matcher = new MethodNameMatcher(typeof (ExampleContext).GetMethod(methodName));
            var def = new StepDefinition {DeclaringType = typeof (ExampleContext), Matcher = matcher};
            return Describer.Describe(def);
        }

        class ExampleContext
        {
            public void This_is_an_example() {}

            public void This_is_an_example_with_a_parameter(int parameter)
            {
            }
        }
    }

   
}