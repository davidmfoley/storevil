using System.IO;
using System.Linq;
using StorEvil.Configuration;

namespace StorEvil.Parsing
{
    internal class FileExtensionFilter
    {
        private readonly ConfigSettings _settings;

        public FileExtensionFilter(ConfigSettings settings)
        {
            _settings = settings;
        }

        public bool IsValid(string file)
        {
            if (_settings.ScenarioExtensions == null || !_settings.ScenarioExtensions.Any())
                return true;

            var extension = Path.GetExtension(file);
            return _settings.ScenarioExtensions.Any(x => extension == x);
        }
    }
}