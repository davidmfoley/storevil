using System;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    [TestFixture]
    public class Inplace_assembly_generation
    {
        private AssemblyGenerator Generator;

        [SetUp]
        public void SetupContext()
        {
            Generator = new AssemblyGenerator();
        }

        [Test]
        public void Should_compile()
        {
            Generator.GenerateAssembly(new Story[0]).ShouldNotBeNull();
        }     

        [Test]
        public void Should_have_a_driver_class()
        {
            Type driverType = GetDriverType();
            driverType.ShouldNotBeNull();
        }

        private Type GetDriverType()
        {
            var assembly = Generator.GenerateAssembly(new[] {new Story("foo", "bar", new IScenario[0]),});
            return assembly.GetType("StorEvilTestAssembly.StorEvilDriver");
        }

        [Test]
        public void Driver_class_exposes_an_Execute_method()
        {
            GetDriverType().GetMethod("Execute").ShouldNotBeNull();
        }
    }
}