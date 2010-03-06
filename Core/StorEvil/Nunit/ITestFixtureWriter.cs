namespace StorEvil.Nunit
{
    public interface ITestFixtureWriter
    {
        void WriteFixture(string storyId, string sourceCode);
        void Finished();
    }
}