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
            var generator = new SparkReportGenerator<GatheredResultSetView>(_fileWriter, _pathToTemplateFile);
            generator.Handle(result);
        }
    }

    public abstract class GatheredResultSetView : AbstractSparkView
    {
        public GatheredResultSet Model { get; set; }
    }
}