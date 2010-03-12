using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil;

namespace Bowling.Context
{
    [Context]
    internal class BowlingContext
    {
        private BowlingGame _game;

        public void Given_a_new_bowling_game()
        {
            // note: Since this class will be instantiated
            // once for each scenario anyway, 
            // this is not really necessary (could be moved to the declaration)
            _game = new BowlingGame();
        }

        public void When_I_roll_the_following_series(int[] rolls)
        {
            foreach (var roll in rolls)
            {
                _game.Roll(roll);
            }
        }

        public void When_I_roll(int pins)
        {
            _game.Roll(pins);
        }

        public void When_my_rolls_are(Roll[] rollSeries)
        {
            foreach (var roll in rollSeries)
            {
                _game.Roll(roll.Pins);
            }
        }
        public void When_I_roll_first_and_second(int first, int second)
        {
            _game.Roll(first);
            _game.Roll(second);
        }

        public void When_I_roll_reps_times_first_and_second(int reps, int first, int second)
        {
            for (int i = 0; i < reps; i++)
            {
                _game.Roll(first);
                _game.Roll(second);
            }            
        }

        public void When_all_of_my_balls_are_landing_in_the_gutter()
        {
            for (int i = 0; i < 20; i++)
            {
                _game.Roll(0);
            }
        }

        public void When_all_of_my_rolls_are_strikes()
        {
            for (int i = 0; i < 12; i++)
            {
                _game.Roll(10);
            }
        }

        public void Then_my_total_score_should_be(int totalScore)
        {
            Assert.That(_game.Score, Is.EqualTo(totalScore)); 
        }
    }

    public class Roll
    {
        public int Pins { get; set; }
    }
}