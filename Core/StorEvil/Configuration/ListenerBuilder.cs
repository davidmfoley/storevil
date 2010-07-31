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

            if (_settings.ConsoleMode != ConsoleMode.Quiet)
            {
                compositeListener.AddListener(new ConsoleResultListener
                                                  {
                                                      ColorEnabled = _settings.ConsoleMode == ConsoleMode.Color
                                                  });
            }

            AddFileWritingListenerIfConfigured(compositeListener);

            return compositeListener;
        }

        private void AddFileWritingListenerIfConfigured(CompositeListener compositeListener)
        {
            if (string.IsNullOrEmpty(_settings.OutputFileFormat))
                return;

            if (string.IsNullOrEmpty(_settings.OutputFile))
                return;

            var outputFile = _settings.OutputFile;

            var fileWriter = new FileWriter(outputFile, true);

            switch (_settings.OutputFileFormat.ToLower())
            {
                case "xml":
                    compositeListener.AddListener(new XmlReportListener(fileWriter));
                    break;
                case "spark":
                    compositeListener.AddListener(new SparkReportListener(fileWriter,
                                                                          _settings.OutputFileTemplate));
                    break;
                default:
                    throw new ConfigurationErrorsException(
                        string.Format("'{0} is not a valid output file format.'", _settings.OutputFileFormat));
            }
        }
    }
}