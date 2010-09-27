using System.IO;
using Spark;
using StorEvil.Infrastructure;
using StorEvil.Spark;

namespace StorEvil.ResultListeners
{
    public class SparkResultReportGenerator : IGatheredResultHandler
    {
        private readonly ITextWriter _fileWriter;
        private readonly string _pathToTemplateFile;

        public SparkResultReportGenerator(ITextWriter fileWriter, string pathToTemplateFile)
        {
            _fileWriter = fileWriter;
            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(GatheredResultSet result)
        {
            
            if (string.IsNullOrEmpty(_pathToTemplateFile))
            {
                CreateReportWithDefaultTemplate(result);
                return;
            }

            var generator = new SparkReportGenerator<GatheredResultSetView>(_fileWriter, _pathToTemplateFile);
            generator.Handle(result);
        }

        private void CreateReportWithDefaultTemplate(GatheredResultSet result)
        {
            var fileName = Path.GetTempFileName();
            var fs = new Filesystem();
            try
            {
                new ResourceWriter(fs).WriteResource("DefaultSparkTemplate.spark", fileName, true);
                var generator = new SparkReportGenerator<GatheredResultSetView>(_fileWriter, fileName);
                generator.Handle(result);
            }
            finally
            {
                fs.Delete(fileName);
            }
        }
    }

    public abstract class GatheredResultSetView : AbstractSparkView
    {
        public GatheredResultSet Model { get; set; }
    }
}