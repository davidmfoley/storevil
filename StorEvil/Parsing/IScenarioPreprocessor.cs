using System.Collections.Generic;

namespace StorEvil.Core
{
    public interface IScenarioPreprocessor
    {
        IEnumerable<Scenario> Preprocess(IScenario scenario);
    }
}