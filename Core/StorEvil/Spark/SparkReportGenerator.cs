using System;
using System.IO;
using System.Text;
using Spark;
using Spark.FileSystem;
using StorEvil.Infrastructure;
using StorEvil.Interpreter;
using StorEvil.ResultListeners;
using StorEvil.Utility;

namespace StorEvil.Spark
{


    public class SparkReportGenerator<TView> where TView :ISparkView
    {
        private readonly ITextWriter _fileWriter;
        private readonly string _pathToTemplateFile;

        public SparkReportGenerator(ITextWriter fileWriter, string pathToTemplateFile)
        {
            _fileWriter = fileWriter;

            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(object model)
        {
            // Find the full path to the template file, 
            // using current directory if argument isn't fully qualified
            string templatePath = GetTemplatePath();

            DebugTrace.Trace(this, "Rendering:" + templatePath);
            DebugTrace.Trace(this, "Model:" + model.ToString());

            var templateName = Path.GetFileName(templatePath);
            var templateDirPath = Path.GetDirectoryName(templatePath);

            SparkViewEngine engine = new SparkEngineFactory().GetSparkEngine<TView>(templateDirPath);

            try
            {
                // compile and instantiate the template
                string templateResult = ProcessViewTemplate(engine, templateName, model);

                _fileWriter.Write(templateResult);
            }
            catch (Exception ex)
            {
                throw new SparkReportGenerationException("An exception occurred:" + ex.GetType().Name + "\r\n" +
                                                         ex.Message, ex);
            }
        }

        private string GetTemplatePath()
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, _pathToTemplateFile);
            if (!File.Exists(templatePath))
                throw new TemplateNotFoundException(templatePath);
            return templatePath;
        }

        private string ProcessViewTemplate(ISparkViewEngine engine, string templateName, object model)
        {
            var view = (TView) engine.CreateInstance(
                new SparkViewDescriptor()
                    .AddTemplate(templateName));

            view.ReflectionSet("Model", model);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                view.RenderView(writer);
            }

            return sb.ToString();
        }
    }

    class SparkEngineFactory
    {
        public SparkViewEngine GetSparkEngine<TView>(string templateDirPath) where TView : ISparkView
        {
            var viewFolder = new FileSystemViewFolder(templateDirPath);

            // Create an engine using the templates path as the root location
            // as well as the shared location
            return new SparkViewEngine
            {
                DefaultPageBaseType = typeof(TView).FullName,
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

    
}