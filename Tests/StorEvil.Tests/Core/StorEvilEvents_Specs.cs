using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.Core.Event_Handling
{

    [TestFixture]
    public class Event_handling
    {

        private EventBus Bus;
        [SetUp]
        public void SetUpContext()
        {
            Bus = new EventBus();
        }

        [Test]
        public void can_register_for_an_event()
        {
            FooHandler handler = new FooHandler();
            Bus.Register(handler);
            var fooEvent = new FooEvent();
            Bus.Raise(fooEvent);
            handler.EventsCaught.Count.ShouldEqual(1);
            handler.EventsCaught.First().ShouldEqual(fooEvent);
        }

        [Test]
        public void Can_register_multiple_handlers()
        {
            var handler1 = new FooHandler();
            var handler2 = new FooHandler();
            Bus.Register(handler1);
            Bus.Register(handler2);
            var fooEvent = new FooEvent();
            Bus.Raise(fooEvent);
            handler1.EventsCaught.Count.ShouldEqual(1);
            handler1.EventsCaught.First().ShouldEqual(fooEvent);
            handler2.EventsCaught.Count.ShouldEqual(1);
            handler2.EventsCaught.First().ShouldEqual(fooEvent);
        }

        [Test]
        public void Raising_an_unhandled_event_does_not_cause_an_error()
        {
            var fooEvent = new FooEvent();
            Bus.Raise(fooEvent);
        }

        [Test]
        public void One_handler_can_handle_multiple_events()
        {
            var handler = new FooBarBazHandler();
            Bus.Register(handler);
            var fooEvent = new FooEvent();
            var barEvent = new BarEvent();
            var bazEvent = new BazEvent();
            Bus.Raise(fooEvent);
            Bus.Raise(barEvent);
            Bus.Raise(bazEvent);
            handler.EventsCaught.ElementsShouldEqual(fooEvent, barEvent, bazEvent);
        }

        [Test] 
        public void Multiple_handlers_handler_can_handle_an_event()
        {
            var fooBarBazHandler = new FooBarBazHandler();
            var fooHandler = new FooHandler();

            Bus.Register(fooBarBazHandler);
            Bus.Register(fooHandler);

            var fooEvent = new FooEvent();
            var barEvent = new BarEvent();
            var bazEvent = new BazEvent();
            
            Bus.Raise(fooEvent);
            Bus.Raise(barEvent);
            Bus.Raise(bazEvent);

            fooBarBazHandler.EventsCaught.ElementsShouldEqual(fooEvent, barEvent, bazEvent);
            fooHandler.EventsCaught.ElementsShouldEqual(fooEvent);
        }

        public class FooHandler : IEventHandler<FooEvent>
        {
            public List<object> EventsCaught = new List<object>();
            public void Handle(FooEvent eventToHandle)
            {
                EventsCaught.Add(eventToHandle);
            }
        }
    }

    public class FooBarBazHandler : IEventHandler<FooEvent, BarEvent, BazEvent>
    {
        public List<object> EventsCaught = new List<object>();
        public void Handle(FooEvent eventToHandle)
        {
            EventsCaught.Add(eventToHandle);
        }

        public void Handle(BarEvent eventToHandle)
        {
            EventsCaught.Add(eventToHandle);
        }

        public void Handle(BazEvent eventToHandle)
        {
            EventsCaught.Add(eventToHandle);
        }
    }

    public class BazEvent
    {
    }

    public class BarEvent
    {
    }


    public class FooEvent
    {
    }

 
}