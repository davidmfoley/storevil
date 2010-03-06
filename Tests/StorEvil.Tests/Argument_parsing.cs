using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Configuration;
using StorEvil.Console;
using StorEvil.InPlace;
using StorEvil.ResultListeners;

namespace StorEvil.Argument_parsing
{
    public class Argument_parsing
    {
        protected ArgParser Parser;
        protected IConfigSource FakeConfigSource;

        [SetUp]
        public void SetupContext()
        {
            FakeConfigSource = MockRepository.GenerateStub<IConfigSource>();
            var settings = new ConfigSettings { AssemblyLocations = new[] { Assembly.GetExecutingAssembly().Location } };

            FakeConfigSource.Stub(x => x.GetConfig("")).IgnoreArguments().Return(settings);
            Parser = new ArgParser(FakeConfigSource);
        }
    }

    [TestFixture]
    public class Building_command_from_args : Argument_parsing
    {
        [Test]
        public void can_create_inplace_job_with_path()
        {
            var result = Parser.ParseArguments(new[] {"execute", Assembly.GetExecutingAssembly().Location});
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_inplace_job_with_no_path()
        {
            var result = Parser.ParseArguments(new[] { "execute" });
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
            result.ShouldBeOfType<DisplayHelpJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void when_xml_output_is_chosen_creates_an_xml_listener()
        {
            var result =
                Parser.ParseArguments(new[]
                                          {
                                             "execute",  "-o", "foo.xml", "-f", "xml"
                                          });

            var composite = Parser.Container.Resolve<IResultListener>() as CompositeListener;
            var xmlListener = composite.Listeners.OfType<XmlReportListener>().FirstOrDefault();
            xmlListener.ShouldNotBeNull();

        }
    }
    
}