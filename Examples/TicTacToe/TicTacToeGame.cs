using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public class TicTacToeGame
    {
        public string[][] BoardState { get; set; }

        public TicTacToeGame()
        {
            BoardState = new[]
                             {
                                 new[] { " ", " ", " " },
                                 new[] { " ", " ", " " },
                                 new[] { " ", " ", " " }
                             };
        }

        public TicTacToeGame(string[][] boardState)
        {
            BoardState = boardState;
        }

        public void PlayMove(int rowIndex, int columnIndex, string player)
        {
            BoardState[rowIndex][columnIndex] = player;
        }

        public string CurrentPlayer
        {
            get { 
                var squaresFilled = BoardState.SelectMany(x => x).Count(IsFilled);
                if (squaresFilled % 2 == 0)
                    return "X";

                return "O";
            }
        }

        public string Winner
        {
            get
            {
                foreach (var line in GetLines())
                {
                    if (IsFilled(line[0]) && line[0] == line[1] && line[1] == line[2])
                    {
                        return line[0];
                    }
                }

                return null;
            }
        }

        private static bool IsFilled(string s)
        {
            return s == "X" || s == "O";
        }

        private IEnumerable<string[]> GetLines()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new[] {BoardState[i][0], BoardState[i][1], BoardState[i][2]};
                yield return new[] {BoardState[0][i], BoardState[1][i], BoardState[2][i]};  
            }

            yield return new[] {BoardState[0][0], BoardState[1][1], BoardState[2][2]};
            yield return new[] { BoardState[2][0], BoardState[1][1], BoardState[0][2] };
        }

       
    }
}