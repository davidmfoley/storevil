using System.Configuration;
using StorEvil.Configuration;
using StorEvil.Infrastructure;
using StorEvil.InPlace;
using StorEvil.ResultListeners;

namespace StorEvil.Console
{
    public class ListenerBuilder
    {
        private readonly ConfigSettings _settings;

        public ListenerBuilder(ConfigSettings settings)
        {
            _settings = settings;
        }

        public IResultListener GetResultListener()
        {
            var compositeListener = new CompositeListener();

            if (!_settings.Quiet)
            {
                compositeListener.AddListener(new ConsoleResultListener
                                                  {
                                                      ColorEnabled = _settings.ConsoleMode == ConsoleMode.Color
                                                  });
            }

            if (!string.IsNullOrEmpty(_settings.OutputFileFormat))
            {
                string outputFile;
                if (!string.IsNullOrEmpty(_settings.OutputFile))
                    outputFile = _settings.OutputFile;
                else
                    outputFile = "storevil-output." + _settings.OutputFileFormat.ToLower();

                var fileWriter = new FileWriter(outputFile, true);

                switch (_settings.OutputFileFormat.ToLower())
                {
                    case "xml":
                        compositeListener.AddListener(new XmlReportListener(fileWriter));
                        break;
                    case "spark":
                        compositeListener.AddListener(new SparkReportListener(fileWriter, _settings.OutputFileTemplate));
                        break;
                    default:
                        throw new ConfigurationErrorsException(string.Format("'{0} is not a valid output file format.'", _settings.OutputFileFormat));
                }
            }

            return compositeListener;
        }
    }
}