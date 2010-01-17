using NUnit.Framework;
using Rhino.Mocks;

namespace StorEvil.Config
{
    [TestFixture]
    public class parsing_valid_config : parsing_config
    {
        const string TestConfigContents = @"
Assemblies: C:\foo\bar\baz.dll,C:\baz\foo.dll
Extensions: .scenario, .foo, .bar";

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
    }   

    public abstract class parsing_config
    {
        protected ConfigSettings ParseConfig(string config)
        {
            const string TestFileName = "C:\\foo\\bar\\storevil.config";

            var fakeFilesystem = MockRepository.GenerateStub<IFilesystem>();
            var parser = new ConfigParser(fakeFilesystem);

            fakeFilesystem
                .Stub(x => x.GetFileText(TestFileName))
                .Return(config);

            return parser.Read(TestFileName);
        }
    }
}