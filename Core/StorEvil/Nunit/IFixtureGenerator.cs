using StorEvil.Core;

namespace StorEvil.Nunit
{
    public interface IFixtureGenerator
    {
        string GenerateFixture(Story story, StoryContext context);
    }
}