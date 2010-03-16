using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil;
using StorEvil.Utility;

namespace TicTacToe
{
    [Context]
    public class StubContextClass
    {
        private TicTacToeGame _game;
        // Given a new game
        public void Given_a_new_game()
        {
            _game = new TicTacToeGame();
        }

        // Given the following board
        public void Given_the_following_board(string[][] tableData)
        {
            _game = new TicTacToeGame(tableData);
        }

        // Then the board state should be
        // And the board state should be
        public void Then_the_board_state_should_be(string[][] tableData)
        {
            for (int row = 0; row < 3; row++)
            {
                _game.BoardState[row].ElementsShouldEqual(tableData[row]);
            }
        }

        // Then the current player should be ...
        public void Then_the_current_player_should_be(string expected)
        {
            _game.CurrentPlayer.ShouldEqual(expected);
        }

        // Then the winner should be ...
        public void Then_the_winner_should_be(string expected)
        {
            _game.Winner.ShouldEqual(expected);
        }

        // Then it should be a cat's game
        public void Then_it_should_be_a_cats_game()
        {
            _game.Winner.ShouldBeNull();
        }

        // When ... plays in the ... ...
        public void When_player_plays_in_the_row_column(string player, string row, string column)
        {
            var rowIndex = Array.IndexOf(new[] {"top", "middle", "bottom"}, row);
            var columnIndex = Array.IndexOf(new[] {"left", "middle", "right"}, column);
            _game.PlayMove(rowIndex, columnIndex, player);
        }
    }
}