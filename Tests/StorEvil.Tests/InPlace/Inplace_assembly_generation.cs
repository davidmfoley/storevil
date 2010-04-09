using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.NUnit;
using StorEvil.Parsing;
using StorEvil.ResultListeners;
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
            GeneratedAssemblyPath = Generator.GenerateAssembly(new Story("foo", "bar", _scenarios), _scenarios.Cast<Scenario>(),
                                                           new[] {this.GetType().Assembly.Location});
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
                "StorEvilTestAssembly.StorEvilDriver", true, 0, null, new object[] {new RemoteListener(null)},CultureInfo.CurrentCulture, new object[0], AppDomain.CurrentDomain.Evidence );

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