using StorEvil;

namespace Tutorial
{
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