using StorEvil;
using StorEvil.Assertions;
using StorEvil.Utility;

namespace Tutorial
{
    [Context]
    public class BasicGrammarContext
    {
        private string _toolName;

        public void When_my_name_is(string toolName)
        {
            _toolName = toolName;
        }

        public void I_should_eat_all_other_BDD_frameworks()
        {
            AllOtherBddFrameworksAreEaten.ShouldEqual(true);
        }

        public bool AllOtherBddFrameworksAreEaten
        {
            get { return _toolName == "StorEvil"; }
        }
    }

    [Context]
    public class FooContext
    {
        private bool _wasCalled;

        public void using_foo_context()
        {
            
        }

        public void call_an_ambiguous_method()
        {
            _wasCalled = true;
        }
        
        public void method_on_foo_should_have_been_called()
        {
            _wasCalled.ShouldBe(true);
        }

    }

    [Context]
    public class BarContext
    {
        private bool _wasCalled;

        public void using_bar_context()
        {

        }

        public void call_an_ambiguous_method()
        {
            _wasCalled = true;
        }

        public void method_on_bar_should_have_been_called()
        {
            _wasCalled.ShouldBe(true);
        }
    }
}