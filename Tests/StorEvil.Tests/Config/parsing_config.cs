using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Configuration;

namespace StorEvil.Config
{
    [TestFixture]
    public class parsing_valid_config : parsing_config
    {
        const string TestConfigContents = @"
Assemblies: C:\foo\bar\baz.dll,C:\baz\foo.dll
Extensions: .scenario, .foo, .bar
OutputFile: foo.html
OutputFileFormat: spark
OutputFileTemplate: foo.spark";

        protected ConfigSettings Result;

        [SetUp]
        public void SetupContext()
        {
            Result = ParseConfig(TestConfigContents);
        }

        [Test]
        public void parses_assembly()
        {
            Result.AssemblyLocations.ElementsShouldEqual(@"C:\foo\bar\baz.dll", @"C:\baz\foo.dll");
        }

        [Test]
        public void should_parse_scenario_extensions()
        {
            Result.ScenarioExtensions.ElementsShouldEqual(".scenario", ".foo", ".bar");
            
        }

        [Test]
        public void should_parse_output_file()
        {
            Result.OutputFile.ShouldEqual("foo.html");
        }

        [Test]
        public void should_parse_output_file_format()
        {
            Result.OutputFileFormat.ShouldEqual("spark");
        }

        [Test]
        public void should_parse_output_file_template()
        {
            Result.OutputFileTemplate.ShouldEqual("foo.spark");
        }
    }

    [TestFixture]
    public class parsing_invalid_config_setting : parsing_config
    {       
        private BadSettingNameException CaughtException;

        [SetUp]
        public void SetupContext()
        {
            try
            {
                ParseConfig("Foo: some fake value");
            }   
            catch (BadSettingNameException ex)
            {
                CaughtException = ex;
            }
        }

        [Test]
        public void throws_exception()
        {
            Assert.IsNotNull(CaughtException);
        }

        [Test]
        public void exception_has_name_of_bad_config_name()
        {
            CaughtException.SettingName.ShouldEqual("Foo");
        }

        [Test]
        public void exception_toString_contains_name_of_bad_setting()
        {
            CaughtException.ToString().ShouldContain("Foo");
        }
    }   

    public abstract class parsing_config
    {
        protected ConfigSettings ParseConfig(string config)
        {
            var parser = new ConfigParser();

            return parser.Read(config);
        }
    }
}