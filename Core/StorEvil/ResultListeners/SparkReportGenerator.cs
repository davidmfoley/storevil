using System;
using System.IO;
using System.Text;
using Spark;
using Spark.FileSystem;
using StorEvil.Infrastructure;

namespace StorEvil.ResultListeners
{
    public class TemplateNotFoundException : Exception
    {
        public string Path { get; set; }

        public TemplateNotFoundException(string path)
        {
            Path = path;
        }

        public override string Message
        {
            get { return string.Format("Could not find the spark template at: '{0}'", Path); }
        }
    }

    public class SparkReportListener : GatheringResultListener
    {
        public SparkReportListener(ITextWriter fileWriter, string pathToTemplateFile)
            : base(new SparkReportGenerator(fileWriter, pathToTemplateFile))
        {
        }
    }

    public class SparkReportGenerator : IGatheredResultHandler
    {
        private readonly ITextWriter _fileWriter;
        private readonly string _pathToTemplateFile;

        public SparkReportGenerator(ITextWriter fileWriter, string pathToTemplateFile)
        {
            _fileWriter = fileWriter;

            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(GatheredResultSet resultSet)
        {
            // Find the full path to the template file, 
            // using current directory if argument isn't fully qualified
            var templatePath = Path.Combine(Environment.CurrentDirectory, _pathToTemplateFile);
            if (!File.Exists(templatePath))
                throw new TemplateNotFoundException(templatePath);

            var templateName = Path.GetFileName(templatePath);
            var templateDirPath = Path.GetDirectoryName(templatePath);

            SparkViewEngine engine = GetSparkEngine(templateDirPath);

            try
            {
                // compile and instantiate the template
                string templateResult = ProcessViewTemplate(engine, templateName, resultSet);

                _fileWriter.Write(templateResult);
            }
            catch (Exception ex)
            {
                throw new SparkReportGenerationException("An exception occurred:" + ex.GetType().Name + "\r\n" +
                                                         ex.Message); //, ex);
            }
        }

        private string ProcessViewTemplate(ISparkViewEngine engine, string templateName, GatheredResultSet result)
        {
            var view = (SparkView) engine.CreateInstance(
                new SparkViewDescriptor()
                    .AddTemplate(templateName));
            
            view.Model = result;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                view.RenderView(writer);
            }

            return sb.ToString();
        }

        private SparkViewEngine GetSparkEngine(string templateDirPath)
        {
            var viewFolder = new FileSystemViewFolder(templateDirPath);

            // Create an engine using the templates path as the root location
            // as well as the shared location
            return new SparkViewEngine
                       {
                           DefaultPageBaseType = typeof (SparkView).FullName,
                           ViewFolder = viewFolder.Append(new SubViewFolder(viewFolder, "Shared")),
                       };
        }
    }

    public class SparkReportGenerationException : Exception
    {
        public SparkReportGenerationException(string message, Exception inner) : base(message, inner)
        {
        }

        public SparkReportGenerationException(string message) : base(message)
        {
        }
    }

    public abstract class SparkView : AbstractSparkView
    {
        public GatheredResultSet Model { get; set; }
    }
}