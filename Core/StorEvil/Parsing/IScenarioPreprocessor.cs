using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Parsing
{
    public interface IScenarioPreprocessor
    {
        IEnumerable<Scenario> Preprocess(IScenario scenario);
    }
}