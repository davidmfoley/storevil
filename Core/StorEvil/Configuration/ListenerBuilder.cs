using System;
using System.Configuration;
using StorEvil.Configuration;
using StorEvil.Events;
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

        private void AddFileWritingListenerIfConfigured(EventBus bus)
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

                    bus.Register(new XmlReportListener(fileWriter));
                    break;
                case "spark":
                    bus.Register(new SparkReportListener(fileWriter, _settings.OutputFileTemplate));
                    break;
                default:
                    throw new ConfigurationErrorsException(
                        string.Format("'{0} is not a valid output file format.'", _settings.OutputFileFormat));
            }
        }

        public void SetUpListeners(EventBus bus)
        {
            if (_settings.ConsoleMode != ConsoleMode.Quiet)
            {
                var consoleListener = new ConsoleResultListener
                {
                    ColorEnabled = _settings.ConsoleMode == ConsoleMode.Color
                };

                bus.Register(consoleListener);
            }

            AddFileWritingListenerIfConfigured(bus);
        }
    }
}