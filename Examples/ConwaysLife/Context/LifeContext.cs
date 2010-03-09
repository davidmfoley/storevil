using System;
using System.Linq;
using ConwaysLife.Model;
using StorEvil;

namespace ConwaysLife.Context
{
    /// <summary>
    /// This is the context that is used by StorEvil to interpret the plain-text specs in the Stories folder.
    /// </summary>
    [Context]
    public class LifeContext
    {
        private LifeBoard _board;

        public void Given_the_following_Setup(string[][] table)
        {
            _board = new LifeBoard(TransformTableToBooleans(table));
        }

        private static bool[][] TransformTableToBooleans(string[][] table)
        {
            var booleanTable = new bool[table.Length][];

            for (int i = 0; i < table.Length; i++)
                booleanTable[i] = table[i].Select(x => x.Trim() == "x").ToArray();

            return booleanTable;
        }

        public void When_I_evolve_the_board()
        {
            _board.Evolve();
        }

        public void Then_I_should_see_the_following_board(string[][] expectedCellStates)
        {          
            var expected = TransformTableToBooleans(expectedCellStates);

            AssertSameDimensionsAsCurrent(expected);
            AssertSameContentsAsCurrent(expected);
        }

        private void AssertSameContentsAsCurrent(bool[][] expected)
        {
            for (int row = 0; row < +_board.RowCount; row++)
                for (int column = 0; column < _board.ColumnCount; column++)
                    AssertCellMatches(expected, row, column);
        }

        private void AssertSameDimensionsAsCurrent(bool[][] expected)
        {
            _board.RowCount.ShouldEqual(expected.Length);
            _board.ColumnCount.ShouldEqual(expected[0].Length);
        }

        private void AssertCellMatches(bool[][] expected, int row, int column)
        {
            if (_board.CellIsAlive(row, column) != expected[row][column])
                ThrowMismatchedCellException(expected, row, column);
        }

        private void ThrowMismatchedCellException( bool[][] expected, int row, int column)
        {
            const string messageFormat = "Mismatch at Row {0}, Column {1}\r\nExpected: {2} but was: {3}";
            var message = string.Format(messageFormat, row, column, expected[row][column], _board.CellIsAlive(row, column));
            throw new Exception(message);
        }

        public void Then_the_center_cell_should_be(CellState state)
        {
            CenterCellIsAlive().ShouldEqual(state == CellState.Alive);
        }

        public enum CellState
        {
            Alive,
            Dead
        }

        private bool CenterCellIsAlive()
        {
            var currentBoardState = _board.GetBoard();

            var row = (currentBoardState.Length - 1)/2;
            var col = (currentBoardState[0].Length - 1)/2;
            return _board.CellIsAlive(row, col);
        }
    }
}