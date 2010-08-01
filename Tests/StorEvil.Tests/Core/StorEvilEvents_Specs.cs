using NUnit.Framework;
using StorEvil.InPlace;
using StorEvil.Utility;

namespace StorEvil.Core.Event_Handling
{
    [TestFixture]
    public class StorEvilEvents_Specs
    {
        private MatchFoundHandlerArgs ArgsPassed;

        [Test]
        public void When_a_handler_is_attached_it_is_invoked()
        {
            ArgsPassed = null;
            StorEvilEvents.OnMatchFound += MemberInvokerOnMatchFound;
            StorEvilEvents.RaiseMatchFound(this, GetType().GetMethod("ExampleMethod"));
            StorEvilEvents.OnMatchFound -= MemberInvokerOnMatchFound;
            ArgsPassed.Member.Name.ShouldBe("ExampleMethod");           
        }

        [Test]
        public void When_no_handler_is_attached_proceeds_as_normal()
        {
            StorEvilEvents.RaiseMatchFound(this, GetType().GetMethod("ExampleMethod"));
            // (should not throw)
        }

        public void ExampleMethod(int foo) {}

        private void MemberInvokerOnMatchFound(object sender, MatchFoundHandlerArgs args)
        {
            ArgsPassed = args;  
        }
    }
}