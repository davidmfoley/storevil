using StorEvil.Context;
using StorEvil.Core;

namespace StorEvil.NUnit
{
    public interface IFixtureGenerator
    {
        string GenerateFixture(Story story, StoryContext context);
    }
}