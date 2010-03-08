using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Infrastructure;
using StorEvil.ResultListeners;

namespace StorEvil.Reports.HTML
{
    public abstract class HTML_Report
    {       
        protected string Result;

        [SetUp]
        public void SetupContext()
        {
            var fakeFileWriter = new FakeFileWriter();

            string pathToTemplate = Path.GetTempFileName();
            File.WriteAllText(pathToTemplate, GetView());
            try
            {
                var generator = new HtmlReportGenerator(fakeFileWriter, pathToTemplate);

                generator.Handle(GetTestResult());
                Result = fakeFileWriter.Result;
               
            }
            finally
            {
                File.Delete(pathToTemplate);
            }

        }

        protected virtual string GetView()
        {
            return @"
<html>
    <head>
    </head>
    <body>
        <h1>TestReport</h1>
        <div class=""no-stories-found"" if=""!Model.HasAnyStories()"">
            No stories were found.
        </div>
        <div class=""stories-wrapper"" if=""Model.HasAnyStories()"">
            <div each=""var story in Model.Stories"">    
                <h2>${story.Summary}</h2>        
            </div>
        </div>
    </body>
</html>
";
        }

        protected abstract GatheredResultSet GetTestResult();

        [Test]
        public void should_generate_valid_XML()
        {
            Result.ShouldBeValidXml();
            Debug.WriteLine(Result);
        }
    }

    [TestFixture]
    public class With_no_stories : HTML_Report 
    {
        
        protected override GatheredResultSet GetTestResult()
        {
            return new GatheredResultSet();
        }

        [Test]
        public void Should_include_the_no_stories_div()
        {
            Result.FindElement("//div[@class='no-stories-found']").ShouldNotBeNull();
        }

        [Test]
        public void Should_not_include_the_story_wrapper()
        {
            Result.FindElement("//div[@class='stories-wrapper']").ShouldBeNull();
        }
    }

    [TestFixture]
    public class With_two_stories : HTML_Report
    {

        protected override GatheredResultSet GetTestResult()
        {
            var result = new GatheredResultSet();
            result.Add(new StoryResult
           {
               Summary = "first story summary"
           });

            result.Add(new StoryResult
            {
                Summary = "second story summary"
            });

            return result;
        }

        [Test]
        public void should_have_first_story_summary()
        {
            Result.ShouldContain("first story summary");
        }

        [Test]
        public void should_have_second_story_summary()
        {
            Result.ShouldContain("second story summary");
        }

        [Test]
        public void Should_not_contain_no_stories_div()
        {
            Result.FindElement("//div[@class='no-stories-found']").ShouldBeNull();
        }
        [Test]
        public void Should_include_the_story_wrapper()
        {
            Result.FindElement("//div[@class='stories-wrapper']").ShouldNotBeNull();
        }
    }
}