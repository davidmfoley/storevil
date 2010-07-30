using StorEvil;

namespace Tutorial
{
    [Context]
    public class AgeContext
    {
        private int _age;
        // Given I am 42 years old
        public void Given_I_am_age_years_old(int age)
        {
            _age = age;
        }

        // My age in one year should be 43
        public int My_age_in_one_year()
        {
            return (_age + 1);
        }
    }

    [Context(Lifetime = ContextLifetime.Story)]
    public class BackgroundContext
    {
        public void Run_the_background()
        {
            Background_run_count++;
        }

        public int Background_run_count;
        
    }
}