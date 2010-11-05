using StorEvil;
using StorEvil.Assertions;

namespace Example
{
    [Context]
    public class ExampleContext
    {
        private int _first;
        private int _second;

        // StorEvil matches based on the names of the examples
        // this will match "Given some condition"
        public void Given_some_condition()
        {
        }

        // You can also use PascalCase naming.
        // (You cannot mix the two in a single method name however)
        public void WhenITakeAnAction()
        {
        }

        // Capitalization does not matter
        public void When_I_Take_Another_Action()
        {
        }

        // StorEvil can use this, along with its "ShouldBe" extension method,
        // to interpret "Then the result should be XXXXXX"      
        public string Then_the_state()
        {
            return "valid";
        }

        // Parameter names are matched automatically ("first" and "second" in this case)
        public void The_numbers_first_and_second(int first, int second)
        {
            // You can store state in this context class.
            // A new context class instance is created for each scenario

            _first = first;
            _second = second;
        }

        // using the context
        public void should_add_to(int expected)
        {
            (_first + _second).ShouldEqual(expected);
        }

        // You can interpret a table of data as an array
        // The first row of the table is used to figure out the names of the fields or properties (see below)
        public void TheseNumbersShouldAddAsExpected(AdditionTestCase[] testCases)
        {
            foreach (var testCase in testCases)
                (testCase.First + testCase.Second).ShouldEqual(testCase.Expected);
        }
    }

    // these fields are filled in from the table in the example
    // based on the field names in the first row
    public class AdditionTestCase
    {
        // can use fields 
        public int First;
        // ... or properties
        public int Second { get; set; }

        public int Expected;
    }
}