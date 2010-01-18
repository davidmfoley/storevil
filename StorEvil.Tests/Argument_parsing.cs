using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Console;

namespace StorEvil.Argument_parsing
{
    public class Argument_parsing
    {
        protected ArgParser Parser;

        [SetUp]
        public void SetupContext()
        {
            Parser = new ArgParser();
        }
    }

    [TestFixture]
    public class Building_command_from_args : Argument_parsing
    {
        [Test]
        public void can_create_inplace_job()
        {
            var result = Parser.ParseArguments(new[] {"execute", Assembly.GetExecutingAssembly().Location});
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_nunit_job()
        {
            var result =
                Parser.ParseArguments(new[]
                                          {
                                              "nunit", Assembly.GetExecutingAssembly().Location,
                                              Directory.GetCurrentDirectory(), Path.GetTempFileName()
                                          });
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_help_job()
        {
            var result =
                Parser.ParseArguments(new[]
                                          {
                                              "help", Assembly.GetExecutingAssembly().Location,
                                              Directory.GetCurrentDirectory(), Path.GetTempFileName()
                                          });
            result.ShouldBeOfType<DisplayHelpJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void displays_usage_if_not_a_recognized_command()
        {
            var result =
                Parser.ParseArguments(new[]
                                          {
                                              "foobar", Assembly.GetExecutingAssembly().Location,
                                              Directory.GetCurrentDirectory(), Path.GetTempFileName()
                                          });
            result.ShouldBeOfType<DisplayUsageJob>();
            result.ShouldNotBeNull();
        }
    }

    
}