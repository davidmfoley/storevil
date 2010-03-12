using System;
using System.Diagnostics;

namespace Bowling.Context
{
    internal class BowlingGame
    {
        private bool IsFirstRollOfFrame = true;
        private int Frame = 1;
        private int LastRoll;
        private int[] RollMultiplier = new int[]{1,1};
        private bool LastRollWasMark;

        public BowlingGame()
        {
            Score = 0;
        }

        public void Roll(int pins)
        {
            var currentRollMultiplier = RollMultiplier[0];
            Score += pins * currentRollMultiplier;

            RollMultiplier = new [] { RollMultiplier[1], 1};           

            if (IsStrike(pins))
            {
                LastRollWasMark = true;               
                if (Frame != 10 && IsFirstRollOfFrame)
                {
                    RollMultiplier[0]++;
                    RollMultiplier[1]++;
                    NextFrame();
                }
            }
            else if (IsSpare(pins) && Frame != 10)
            {
                RollMultiplier[0]++;
                LastRollWasMark = true;
                NextFrame();
            }
            else if (!IsFirstRollOfFrame)
            {
                NextFrame();
                LastRollWasMark = false;
            }
            else
            {
                IsFirstRollOfFrame = false;
                LastRollWasMark = false;
            }
            LastRoll = pins;
        }

        private void NextFrame()
        {
            Frame++;
            IsFirstRollOfFrame = true;
        }

        private bool IsSpare(int pins)
        {
            return !IsFirstRollOfFrame && (pins + LastRoll) == 10;
        }

        private bool IsStrike(int pins)
        {
            return (IsStrikeRoll() && pins == 10);
        }

        private bool IsStrikeRoll()
        {
            if (Frame == 10)
                return IsFirstRollOfFrame || LastRollWasMark;

            return IsFirstRollOfFrame;
        }

        public int Score { get; private set; }
    }
}