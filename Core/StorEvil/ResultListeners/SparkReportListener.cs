using StorEvil.Infrastructure;
using StorEvil.Spark;

namespace StorEvil.ResultListeners
{
    public class SparkReportListener : GatheringResultListener
    {
        public SparkReportListener(ITextWriter fileWriter, string pathToTemplateFile)
            : base(new SparkResultReportGenerator(fileWriter, pathToTemplateFile))
        {
        }
    }
}