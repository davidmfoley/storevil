using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Infrastructure;

namespace StorEvil.Console
{
    [TestFixture]
    public class InitJob_Specs
    {
        private IFilesystem Filesystem;
        private InitJob Job;

        [SetUp]
        public void SetupContext()
        {
            Filesystem = MockRepository.GenerateMock<IFilesystem>();
            Job = new InitJob(Filesystem);
            Job.Run();
        }

        [Test]
        public void Writes_config()
        {
            AssertFileWritten("storevil.config");
        }

        [Test]
        public void Writes_spark()
        {
            AssertFileWritten("default.spark");
        }

        private void AssertFileWritten(string name)
        {
            Filesystem.AssertWasCalled(x => x.WriteFile(Arg<string>.Matches(n => n.EndsWith(name)), Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }
    }
}