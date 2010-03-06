using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using StorEvil.Core;
using StorEvil.InPlace;

namespace StorEvil.Reports.XmlReportListener_Specs
{
    [TestFixture]
    public abstract class Xml_reports
    {
        protected XmlReportListener Writer;
        protected FakeFileWriter FileWriter;
        protected string Result;

        [SetUp]
        public void SetupContext()
        {
            FileWriter = new FakeFileWriter();

            Writer = new XmlReportListener(FileWriter);

            DoTestSetup(Writer);
            Result = FileWriter.Result;
            Debug.WriteLine(Result);
        }

        protected IEnumerable<XmlElement> StoryElements()
        {
            return Result.FindElements("//Story");
        }

        protected XmlElement FirstStoryElement()
        {
            return Result.FindElement("//Story");
        }

        protected IEnumerable<XmlElement> AllScenarioElements()
        {
            return Result.FindElements("//Story/Scenario");
        }

        protected XmlElement FirstScenarioElement()
        {
            return Result.FindElement("//Story/Scenario");
        }

        protected static void CheckStatus(XmlElement xmlElement, string status)
        {
            xmlElement.GetAttribute("Status").ShouldEqual(status);
        }

        protected IEnumerable<XmlElement> GetFirstScenarioLines()
        {
            return FirstScenarioElement().FindElements("./Line");
        }

        protected abstract void DoTestSetup(XmlReportListener writer);
    }

    [TestFixture]
    public class Writing_story_with_no_scenarios : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            Writer.StoryStarting(new Story("id", "summary", new IScenario[0]));
            Writer.Finished();
        }

        [Test]
        public void Generates_valid_xml()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void Has_a_story_node()
        {
            StoryElements().Count().ShouldEqual(1);
        }

        [Test]
        public void Story_node_has_id()
        {
            FirstStoryElement().GetAttribute("Id").ShouldEqual("id");
        }

        [Test]
        public void Story_node_has_summary()
        {
            FirstStoryElement().GetAttribute("Summary").ShouldEqual("summary");
        }

        [Test]
        public void No_scenarios_are_output()
        {
            AllScenarioElements().Any().ShouldEqual(false);
        }
    }

    [TestFixture]
    public class Writing_xml_for_a_story_with_one_successful_scenario : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            Scenario testScenario = new Scenario("scenarioId", "scenarioName", new[] {"line1", "line2"});
            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {testScenario}));
            Writer.ScenarioStarting(testScenario);
            Writer.Success(testScenario, "line1");
            Writer.Success(testScenario, "line2");
            Writer.ScenarioSucceeded(testScenario);
            Writer.Finished();
        }

        [Test]
        public void Generates_valid_XML()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void Includes_one_story_tag()
        {
            StoryElements().Count().ShouldEqual(1);
        }

        [Test]
        public void Story_is_marked_successful()
        {
            FirstStoryElement().GetAttribute("Status").ShouldEqual("Success");
        }

        [Test]
        public void Has_a_scenario_tag()
        {
            AllScenarioElements().Count().ShouldEqual(1);
        }

        [Test]
        public void Scenario_tag_is_marked_successful()
        {
            FirstScenarioElement().GetAttribute("Status").ShouldEqual("Success");
        }

        [Test]
        public void line_elements_exist_in_scenario_text()
        {
            GetFirstScenarioLines().Count().ShouldEqual(2);
        }

        [Test]
        public void line_elements_should_be_marked_successful()
        {
            CheckStatus(GetFirstScenarioLines().First(), "Success");
            CheckStatus(GetFirstScenarioLines().Last(), "Success");
        }
    }

    [TestFixture]
    public class Writing_xml_for_a_story_with_one_unsuccessful_scenario : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var testScenario = new Scenario("scenarioId", "scenarioName", new[] {"line1", "line2"});
            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {testScenario}));
            Writer.ScenarioStarting(testScenario);
            Writer.Success(testScenario, "line1");
            Writer.ScenarioFailed(testScenario, "successPart", "failedPart", "failureMessage");
            Writer.Finished();
        }

        [Test]
        public void Generates_valid_XML()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void Story_is_marked_as_unsuccessful()
        {
            CheckStatus(FirstStoryElement(), "Failure");
        }

        [Test]
        public void Scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(FirstScenarioElement(), "Failure");
        }

        [Test]
        public void line_elements_exist_in_scenario_text()
        {
            GetFirstScenarioLines().Count().ShouldEqual(3);
        }

        [Test]
        public void first_and_second_line_elements_should_be_marked_successful()
        {
            CheckStatus(GetFirstScenarioLines().First(), "Success");
            CheckStatus(GetFirstScenarioLines().ElementAt(1), "Success");
        }

        [Test]
        public void Last_line_element_is_marked_as_failure()
        {
            CheckStatus(GetFirstScenarioLines().Last(), "Failure");
        }
    }

    [TestFixture]
    public class Writing_xml_for_a_story_with_one_successful_and_one_unsuccessful_scenario : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var sucessScenario = new Scenario("successId", "successName", new[] {"successLine"});
            var failureScenario = new Scenario("failureId", "failureName", new[] {"failureLine"});

            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario, failureScenario}));

            Writer.ScenarioStarting(sucessScenario);
            Writer.Success(sucessScenario, "line1");
            Writer.ScenarioSucceeded(sucessScenario);

            Writer.ScenarioStarting(failureScenario);
            Writer.ScenarioFailed(failureScenario, "foo", "bar", "failed");

            Writer.Finished();
        }

        [Test]
        public void Generates_valid_XML()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void There_is_one_story_element()
        {
            StoryElements().Count().ShouldEqual(1);
        }

        [Test]
        public void Story_has_two_scenarios()
        {
            FirstStoryElement().FindElements("./Scenario").Count().ShouldEqual(2);
        }

        [Test]
        public void Story_is_marked_as_unsuccessful()
        {
            CheckStatus(FirstStoryElement(), "Failure");
        }

        [Test]
        public void First_scenario_is_marked_as_successful()
        {
            CheckStatus(FirstScenarioElement(), "Success");
        }

        [Test]
        public void Second_scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(AllScenarioElements().Last(), "Failure");
        }
    }

    [TestFixture]
    public class Writing_xml_for_multiple_stories : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var sucessScenario = new Scenario("successId", "successName", new[] {"successLine"});

            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario}));

            Writer.ScenarioStarting(sucessScenario);
            Writer.Success(sucessScenario, "line1");
            Writer.ScenarioSucceeded(sucessScenario);

            var failureScenario = new Scenario("failureId", "failureName", new[] {"failureLine"});

            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {failureScenario}));

            Writer.ScenarioStarting(failureScenario);
            Writer.ScenarioFailed(failureScenario, "foo", "bar", "failed");

            Writer.Finished();
        }

        [Test]
        public void Generates_valid_XML()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void There_are_two_story_element()
        {
            StoryElements().Count().ShouldEqual(2);
        }

        [Test]
        public void First_story_is_marked_as_successful()
        {
            CheckStatus(FirstStoryElement(), "Success");
        }

        [Test]
        public void Second_story_is_marked_as_unsuccessful()
        {
            CheckStatus(StoryElements().Last(), "Failure");
        }

        [Test]
        public void First_scenario_is_marked_as_successful()
        {
            CheckStatus(FirstScenarioElement(), "Success");
        }

        [Test]
        public void Second_scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(AllScenarioElements().Last(), "Failure");
        }
    }

    [TestFixture]
    public class Writing_xml_when_line_is_not_interpreted : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var line = "!<%&>/>\"'line1";
            var scenario = new Scenario("<successId&>\"'", "<successName>%&\"'", new[] {line});

            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {scenario}));

            Writer.ScenarioStarting(scenario);

            Writer.Success(scenario, line);
            Writer.CouldNotInterpret(scenario, line);

            Writer.Finished();
        }

        [Test]
        public void Should_be_valid_xml()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void Scenario_should_have_NotUnderstood_status()
        {
            CheckStatus(FirstScenarioElement(), "NotUnderstood");
        }

        [Test]
        public void Last_line_should_have_NotUnderstood_status()
        {
            CheckStatus(FirstScenarioElement().FindElements("./Line").Last(), "NotUnderstood");
        }
    }

    [TestFixture]
    public class Writing_xml_when_special_characters_are_present : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var line = "!<%&>/>\"'line1";
            var sucessScenario = new Scenario("<successId&>\"'", "<successName>%&\"'", new[] {line});

            Writer.StoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario}));

            Writer.ScenarioStarting(sucessScenario);

            Writer.Success(sucessScenario, line);
            Writer.ScenarioFailed(sucessScenario, line, line, line);

            Writer.Finished();
        }

        [Test]
        public void Should_be_valid_xml()
        {
            Result.ShouldBeValidXml();
        }
    }
}