using System.Configuration;
using StorEvil.Events;
using StorEvil.Infrastructure;
using StorEvil.ResultListeners;

namespace StorEvil.Configuration
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
            if (string.IsNullOrEmpty(_settings.OutputFile))
                return;            
           
            bus.Register(GetWriterForOutputFormat());           
        }

        private object GetWriterForOutputFormat()
        {
            var fileWriter = new FileWriter(_settings.OutputFile, true);
            var lowerCase = (_settings.OutputFileFormat ?? "" ).ToLower();
            
            if (lowerCase == "xml")
                return new XmlReportListener(fileWriter);

            if (lowerCase == "spark" || lowerCase == "")
                return new SparkReportListener(fileWriter, _settings.OutputFileTemplate);

            throw new ConfigurationErrorsException(
                       string.Format("'{0} is not a valid output file format.'", _settings.OutputFileFormat));            
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