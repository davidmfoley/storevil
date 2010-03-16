using StorEvil;
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
}