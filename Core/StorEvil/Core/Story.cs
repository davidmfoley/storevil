using System.Collections.Generic;

namespace StorEvil.Core
{
    /// <summary>
    /// A story describes some aspect of behavior of the target system.
    /// 
    /// The scenarios are what are used to actually generate the test code 
    /// </summary>
    public class Story
    {
        public string Id { get; set; }

        public string Summary { get; private set; }

        public Story(string id, string summary, IEnumerable<IScenario> scenarios)
        {
            Summary = summary;
            Id = id;
            Scenarios = scenarios;
        }

        public IEnumerable<IScenario> Scenarios;
    }
}