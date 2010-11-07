using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using StorEvil.Assertions;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.ResultListeners.XmlReportListener_Specs
{
    [TestFixture]
    public abstract class Xml_reports
    {
        protected XmlReportListener Writer;
        protected FakeTextWriter FileWriter;
        protected string Result;

        protected Scenario BuildTestScenario(string id, string name, params string[] body)
        {
            return new Scenario("", id, name, body.Select(x=>new ScenarioLine {Text = x}).ToArray());
        }

        [SetUp]
        public void SetupContext()
        {
            FileWriter = new FakeTextWriter();

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

        protected void SimulateStoryFailed(Scenario scenario, string successPart, string failedPart, string message)
        {
            Writer.Handle(new LineFailed { FailedPart = failedPart, SuccessPart = successPart, ExceptionInfo = message });
            Writer.Handle(new ScenarioFailed { Scenario = scenario });
        }

        protected void SimulateScenarioSucceeded(Scenario scenario)
        {
            Writer.Handle(new ScenarioPassed { Scenario = scenario });
        }

        protected void SimulateSuccessfulLine(Scenario scenario, string line)
        {
            Writer.Handle(new LinePassed { Line = line});
        }

        protected void SimulateScenarioStarting(Scenario scenario)
        {
            Writer.Handle(new ScenarioStarting { Scenario = scenario });
        }

        protected void SimulateStoryStarting(Story story)
        {
            Writer.Handle(new StoryStarting { Story = story });
        }
    }

    [TestFixture]
    public class Writing_story_with_no_scenarios : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            SimulateStoryStarting(new Story("id", "summary", new IScenario[0]));
            Writer.Handle(new SessionFinished());
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
            Scenario testScenario = new Scenario("", "scenarioId", "scenarioName", new[] {"line1", "line2"}.Select(l=>new ScenarioLine {Text = l}).ToArray());
           SimulateStoryStarting(new Story("id", "summary", new IScenario[] {testScenario}));
            SimulateScenarioStarting(testScenario);
            SimulateSuccessfulLine(testScenario, "line1");
            SimulateSuccessfulLine(testScenario, "line2");
            SimulateScenarioSucceeded(testScenario);
            Writer.Handle(new SessionFinished());
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
            FirstStoryElement().GetAttribute("Status").ShouldEqual("Passed");
        }

        [Test]
        public void Has_a_scenario_tag()
        {
            AllScenarioElements().Count().ShouldEqual(1);
        }

        [Test]
        public void Scenario_tag_is_marked_successful()
        {
            FirstScenarioElement().GetAttribute("Status").ShouldEqual("Passed");
        }

        [Test]
        public void line_elements_exist_in_scenario_text()
        {
            GetFirstScenarioLines().Count().ShouldEqual(2);
        }

        [Test]
        public void line_elements_should_be_marked_successful()
        {
            CheckStatus(GetFirstScenarioLines().First(), "Passed");
            CheckStatus(GetFirstScenarioLines().Last(), "Passed");
        }
    }

    [TestFixture]
    public class Writing_xml_for_a_story_with_one_unsuccessful_scenario : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var testScenario = BuildTestScenario("scenarioId", "scenarioName", new[] {"line1", "line2"});
           SimulateStoryStarting(new Story("id", "summary", new IScenario[] {testScenario}));
            SimulateScenarioStarting(testScenario);
            SimulateSuccessfulLine(testScenario, "line1");
            SimulateStoryFailed(testScenario, "successPart", "failedPart", "failureMessage");
            Writer.Handle(new SessionFinished());
        }

        [Test]
        public void Generates_valid_XML()
        {
            Result.ShouldBeValidXml();
        }

        [Test]
        public void Story_is_marked_as_unsuccessful()
        {
            CheckStatus(FirstStoryElement(), "Failed");
        }

        [Test]
        public void Scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(FirstScenarioElement(), "Failed");
        }

        [Test]
        public void line_elements_exist_in_scenario_text()
        {
            GetFirstScenarioLines().Count().ShouldEqual(3);
        }

        [Test]
        public void first_and_second_line_elements_should_be_marked_successful()
        {
            CheckStatus(GetFirstScenarioLines().First(), "Passed");
            CheckStatus(GetFirstScenarioLines().ElementAt(1), "Passed");
        }

        [Test]
        public void Last_line_element_is_marked_as_failure()
        {
            CheckStatus(GetFirstScenarioLines().Last(), "Failed");
        }
    }

    [TestFixture]
    public class Writing_xml_for_a_story_with_one_successful_and_one_unsuccessful_scenario : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var sucessScenario = BuildTestScenario("successId", "successName", new[] { "successLine" });
            var failureScenario = BuildTestScenario("failureId", "failureName", new[] { "failureLine" });

            SimulateStoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario, failureScenario}));

            SimulateScenarioStarting(sucessScenario);
            SimulateSuccessfulLine(sucessScenario, "line1");
            SimulateScenarioSucceeded(sucessScenario);

            SimulateScenarioStarting(failureScenario);
            SimulateStoryFailed(failureScenario, "foo", "bar", "failed");

            Writer.Handle(new SessionFinished());
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
            CheckStatus(FirstStoryElement(), "Failed");
        }

        [Test]
        public void First_scenario_is_marked_as_successful()
        {
            CheckStatus(FirstScenarioElement(), "Passed");
        }

        [Test]
        public void Second_scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(AllScenarioElements().Last(), "Failed");
        }
    }

    [TestFixture]
    public class Writing_xml_for_multiple_stories : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var sucessScenario = BuildTestScenario("successId", "successName", new[] { "successLine" });

            SimulateStoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario}));

            SimulateScenarioStarting(sucessScenario);
            SimulateSuccessfulLine(sucessScenario, "line1");
            SimulateScenarioSucceeded(sucessScenario);

            var failureScenario = BuildTestScenario("failureId", "failureName", new[] { "failureLine" });

            SimulateStoryStarting(new Story("id", "summary", new IScenario[] {failureScenario}));

            SimulateScenarioStarting(failureScenario);
            SimulateStoryFailed(failureScenario, "foo", "bar", "failed");

            Writer.Handle(new SessionFinished());
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
            CheckStatus(FirstStoryElement(), "Passed");
        }

        [Test]
        public void Second_story_is_marked_as_unsuccessful()
        {
            CheckStatus(StoryElements().Last(), "Failed");
        }

        [Test]
        public void First_scenario_is_marked_as_successful()
        {
            CheckStatus(FirstScenarioElement(), "Passed");
        }

        [Test]
        public void Second_scenario_is_marked_as_unsuccessful()
        {
            CheckStatus(AllScenarioElements().Last(), "Failed");
        }
    }

    [TestFixture]
    public class Writing_xml_when_line_is_not_interpreted : Xml_reports
    {
        protected override void DoTestSetup(XmlReportListener writer)
        {
            var line = "!<%&>/>\"'line1";
            var scenario = BuildTestScenario("<successId&>\"'", "<successName>%&\"'", new[] { line });

           SimulateStoryStarting(new Story("id", "summary", new IScenario[] {scenario}));

            SimulateScenarioStarting(scenario);

            SimulateSuccessfulLine(scenario, line);
            Writer.Handle(new LinePending  {Line = line });
            Writer.Handle(new ScenarioPending { Scenario = scenario });
            Writer.Handle(new SessionFinished());
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
            var sucessScenario = BuildTestScenario("<successId&>\"'", "<successName>%&\"'", new[] { line });

           SimulateStoryStarting(new Story("id", "summary", new IScenario[] {sucessScenario}));

            SimulateScenarioStarting(sucessScenario);

            SimulateSuccessfulLine(sucessScenario, line);
            SimulateStoryFailed(sucessScenario, line, line, line);

            Writer.Handle(new SessionFinished());
        }

        [Test]
        public void Should_be_valid_xml()
        {
            Result.ShouldBeValidXml();
        }
    }
}