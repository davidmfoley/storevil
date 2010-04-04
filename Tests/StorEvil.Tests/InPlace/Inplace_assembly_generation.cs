using System;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Inplace_assembly_generation
    {
        private AssemblyGenerator Generator;
        private Assembly GeneratedAssembly;

        [SetUp]
        public void SetupContext()
        {
            Generator = new AssemblyGenerator();
            var scenarios = new IScenario[]
                                {
                                    TestHelper.BuildScenario("foo", "When I do seomthing",
                                                             "something else should happen")
                                };
            GeneratedAssembly = Generator.GenerateAssembly(new Story("foo", "bar", scenarios));
        }

        [Test]
        public void Should_compile()
        {
            GeneratedAssembly.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_a_driver_class()
        {
            GetDriverType().ShouldNotBeNull();
        }

        private Type GetDriverType()
        {
            return GeneratedAssembly.GetType("StorEvilTestAssembly.StorEvilDriver");
        }

        [Test]
        public void Driver_class_exposes_an_Execute_method()
        {
            GetDriverType().GetMethod("Execute").ShouldNotBeNull();
        }
    }
}