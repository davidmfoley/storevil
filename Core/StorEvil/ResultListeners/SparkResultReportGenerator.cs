using StorEvil.Infrastructure;
using StorEvil.ResultListeners;

namespace StorEvil.Spark
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
}