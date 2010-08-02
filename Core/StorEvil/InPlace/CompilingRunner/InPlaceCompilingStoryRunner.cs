using System.Collections.Generic;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Events;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class InPlaceCompilingStoryRunner : InPlaceStoryRunnerBase
    {
        private readonly IRemoteHandlerFactory _factory;   
        

        public InPlaceCompilingStoryRunner(IRemoteHandlerFactory factory, IScenarioPreprocessor preprocessor, IStoryFilter filter, ISessionContext context, IEventBus eventBus)
            : base(preprocessor, filter, context, eventBus)
        {
            
            _factory = factory;
           
        }

        protected override void Execute(Story story, IEnumerable<Scenario> scenarios, StoryContext context)
        {
            using (var remoteHandler = _factory.GetHandler(story, scenarios, EventBus))
            {
                var handler = remoteHandler.Handler;
                handler.HandleStory(story);
                Result += handler.GetResult();
            }
        }
    }
}