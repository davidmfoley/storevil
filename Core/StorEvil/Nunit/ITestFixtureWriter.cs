namespace StorEvil.NUnit
{
    public interface ITestFixtureWriter
    {
        void WriteFixture(string storyId, string sourceCode);
        void Finished();
        void WriteSetUpAndTearDown (string setupTearDown);
    }
}