using System;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public abstract class DriverBase : MarshalByRefObject, IStoryHandler
    {
        protected JobResult Result = new JobResult();

        public abstract void HandleStory(Story story);

        public void Finished()
        {
           
        }

        public JobResult GetResult()
        {
            return Result;
        }
    }
}