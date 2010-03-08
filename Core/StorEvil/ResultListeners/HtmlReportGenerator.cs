using System;
using System.IO;
using System.Text;
using Spark;
using Spark.FileSystem;
using StorEvil.Infrastructure;

namespace StorEvil.ResultListeners
{
    public class HtmlReportGenerator : IGatheredResultHandler
    {
        private readonly IFileWriter _fileWriter;        
        private readonly string _pathToTemplateFile;

        public HtmlReportGenerator(IFileWriter fileWriter,  string pathToTemplateFile)
        {
            _fileWriter = fileWriter;

            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(GatheredResultSet result)
        {
            // Find the full path to the template file, 
            // using current directory if argument isn't fully qualified
            var templatePath = Path.Combine(Environment.CurrentDirectory, _pathToTemplateFile);
            var templateName = Path.GetFileName(templatePath);
            var templateDirPath = Path.GetDirectoryName(templatePath);

            var viewFolder = new FileSystemViewFolder(templateDirPath);

            // Create an engine using the templates path as the root location
            // as well as the shared location
            var engine = new SparkViewEngine
                             {
                                 DefaultPageBaseType = typeof (SparkView).FullName,
                                 ViewFolder = viewFolder.Append(new SubViewFolder(viewFolder, "Shared"))
                             };

            // compile and instantiate the template
            SparkView view = (SparkView) engine.CreateInstance(
                new SparkViewDescriptor()
                    .AddTemplate(templateName));

            view.Model = result;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                view.RenderView(writer);
            }

            _fileWriter.Write(sb.ToString());
        }
    }

    public abstract class SparkView : AbstractSparkView
    {
        public GatheredResultSet Model { get; set; }
    }
}