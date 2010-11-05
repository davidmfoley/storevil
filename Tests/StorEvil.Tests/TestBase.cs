using Rhino.Mocks;

namespace StorEvil
{
    public class TestBase
    {
        protected T Fake<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }
    }
}