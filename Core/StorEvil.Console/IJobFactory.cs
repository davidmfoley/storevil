using Funq;

namespace StorEvil.Console
{
    public interface IJobFactory
    {
        string GetUsage();
        void SetupContainer(Container container, string[] args);
    }
}