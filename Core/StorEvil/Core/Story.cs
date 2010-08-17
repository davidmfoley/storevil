using System;
using System.Collections.Generic;

namespace StorEvil.Core
{ 
    [Serializable]
    public class Story
    {
        public string Id { get; set; }

        public string Summary { get; set; }

        public IEnumerable<string> Tags { get; set;}

        public string Location { get; set; }

        public Story(string id, string summary, IEnumerable<IScenario> scenarios)
        {
            Summary = summary;
            Id = id;
            Scenarios = scenarios;
        }

        public IEnumerable<IScenario> Scenarios;
    }
}