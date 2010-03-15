using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Configuration;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.Interpreter;
using StorEvil.ResultListeners;
using StorEvil.StubGeneration;
using StorEvil.Utility;

namespace StorEvil.Console
{
    public abstract class Job_creation_spec
    {
        protected JobFactory Factory;
        protected IConfigSource FakeConfigSource;

        [SetUp]
        public void SetupContext()
        {
            FakeConfigSource = MockRepository.GenerateStub<IConfigSource>();
            ConfigSettings settings = GetSettings();

            FakeConfigSource.Stub(x => x.GetConfig("")).IgnoreArguments().Return(settings);
            Factory = new JobFactory(FakeConfigSource);
        }

        protected abstract ConfigSettings GetSettings();
    }

    [TestFixture]
    public class Building_command_from_args : Job_creation_spec
    {
        protected override ConfigSettings GetSettings()
        {
            return new ConfigSettings {AssemblyLocations = new[] {Assembly.GetExecutingAssembly().Location}};
        }

        [Test]
        public void can_create_inplace_job_with_path()
        {
            var result = Factory.ParseArguments(new[] {"execute", Assembly.GetExecutingAssembly().Location});
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_inplace_job_with_no_path()
        {
            var result = Factory.ParseArguments(new[] {"execute"});
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_nunit_job()
        {
            var result =
                Factory.ParseArguments(new[]
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
                Factory.ParseArguments(new[]
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
                Factory.ParseArguments(new[]
                                           {
                                               "foobar", Assembly.GetExecutingAssembly().Location,
                                               Directory.GetCurrentDirectory(), Path.GetTempFileName()
                                           });
            result.ShouldBeOfType<DisplayHelpJob>();
            result.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class StubGeneration_job : Job_creation_spec
    {
        [Test]
        public void can_create_generation_job()
        {
            var job = Factory.ParseArguments(new[] { "stub" });
            job.ShouldBeOfType<StorEvilJob>();
            job.ShouldNotBeNull();
        }

        [Test]
        public void Sets_up_story_handler_to_be_generator()
        {
            var job = Factory.ParseArguments(new[] { "stub" });
            StubGenerator generator = GetGenerator();
            generator.ShouldNotBeNull();
        }

        private StubGenerator GetGenerator()
        {
            return Factory.Container.Resolve<IStoryHandler>() as StubGenerator;
        }

        [Test]
        public void Sets_up_clipboard_writer_when_switch_is_set()
        {
            var job = Factory.ParseArguments(new[] { "stub", "-b" });
            GetGenerator().SuggestionWriter.ShouldBeOfType<ClipboardWriter>();

        }

        [Test]
        public void Sets_up_file_writer_when_switch_is_set()
        {
            var job = Factory.ParseArguments(new[] { "stub", "-d", "foo.txt" });
            var writer = GetGenerator().SuggestionWriter as FileWriter;
            writer.ShouldNotBeNull();
            writer.OutputFile.ShouldEqual("foo.txt");
        }


        protected override ConfigSettings GetSettings()
        {
            return new ConfigSettings {AssemblyLocations = new[] {Assembly.GetExecutingAssembly().Location}};
        }
    }

    [TestFixture]
    public class Wiring_up_result_listeners : Job_creation_spec
    {
        protected override ConfigSettings GetSettings()
        {
            return new ConfigSettings {AssemblyLocations = new[] {Assembly.GetExecutingAssembly().Location}};
        }

        [Test]
        public void when_xml_output_is_chosen_creates_an_xml_listener()
        {
            Factory.ParseArguments(new[]
                                       {
                                           "execute", "-o", "foo.xml", "-f", "xml"
                                       });

            var composite = Factory.Container.Resolve<IResultListener>() as CompositeListener;
            var xmlListener = composite.Listeners.OfType<XmlReportListener>().FirstOrDefault();
            xmlListener.ShouldNotBeNull();
        }

        [Test]
        public void when_spark_output_is_chosen_creates_a_spark_listener()
        {
            Factory.ParseArguments(new[]
                                       {
                                           "execute", "-o", "foo.html", "-f", "spark"
                                       });

            var composite = Factory.Container.Resolve<IResultListener>() as CompositeListener;
            var htmlReportGenerator = composite.Listeners.OfType<SparkReportListener>().FirstOrDefault();
            htmlReportGenerator.ShouldNotBeNull();
        }

        [Test]
        public void when_debug_is_not_set_registers_no_op_debug_listener()
        {
            Factory.ParseArguments(new[]
                                       {
                                           "execute"
                                       });

            DebugTrace.Listener.ShouldBeNull();
        }

        [Test]
        public void when_debug_is_set_registers_console_debug_listener()
        {
            Factory.ParseArguments(new[]
                                       {
                                           "execute", "--debug"
                                       });


            
            DebugTrace.Listener.ShouldBeOfType<ConsoleDebugListener>();
        }
    }


}