using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Core;
using StorEvil.InPlace.CompilingRunner;
using StorEvil.NUnit;
using StorEvil.Utility;

namespace StorEvil.InPlace.Compiled
{
    [TestFixture]
    public class Inplace_assembly_generation
    {
        private AssemblyGenerator Generator;
        private string GeneratedAssemblyPath;
        private IScenario[] _scenarios;

        [SetUp]
        public void SetupContext()
        {
            Generator = new AssemblyGenerator();
            _scenarios = new IScenario[]
                             {
                                 TestHelper.BuildScenario("foo", "When I do seomthing",
                                                          "something else should happen")
                             };
            
            var spec = new AssemblyGenerationSpec {Assemblies = new[] {this.GetType().Assembly.Location}};
            spec.AddStory(new Story("foo", "bar", _scenarios), _scenarios);
            
            GeneratedAssemblyPath = Generator.GenerateAssembly(spec);

        }

        [Test]
        public void Should_exist()
        {
            File.Exists(GeneratedAssemblyPath).ShouldBe(true);
        }

        [Test]
        public void Should_be_able_to_instantiate()
        {
            var handle = Activator.CreateInstanceFrom(
                GeneratedAssemblyPath,
                "StorEvilTestAssembly.StorEvilDriver_foo", true, 0, null, new object[] {new CapturingEventBus()},CultureInfo.CurrentCulture, new object[0]);

            var driver = handle.Unwrap() as IStoryHandler;

            driver.ShouldNotBeNull();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            File.Delete(GeneratedAssemblyPath);
        }
    }

  
}