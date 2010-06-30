namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        void HandleStory(Story story);
        void Finished();
        JobResult GetResult();
    }
}