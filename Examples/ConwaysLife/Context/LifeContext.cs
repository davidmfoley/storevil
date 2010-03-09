using System;
using System.Linq;
using ConwaysLife.Model;
using StorEvil;

namespace ConwaysLife.Context
{
    [Context]
    public class LifeContext
    {
        private LifeBoard _board;

        public void Given_the_following_Setup(string[][] table)
        {
            _board = new LifeBoard(TransformTableToBooleans(table));
        }

        private bool[][] TransformTableToBooleans(string[][] table)
        {
            bool[][] booleanTable = new bool[table.Length][];
            for (int i = 0; i < table.Length; i++)
            {
                var row = table[i];
                booleanTable[i] = row.Select(x => x.Trim() == "x").ToArray();
            }

            return booleanTable;
        }

        public void When_I_evolve_the_board()
        {
            _board.Evolve();
        }

        public void Then_I_should_see_the_following_board(string[][] expectedCellStates)
        {
            var actual = _board.GetBoard();
            var expected = TransformTableToBooleans(expectedCellStates);

            actual.Length.ShouldEqual(expected.Length);
            _board.ColumnCount.ShouldEqual(expected[0].Length);

            for (int row = 0; row < expected.Length; row++)
            {
                actual[row].Length.ShouldEqual(expected[row].Length);
                for (int column = 0; column < expected[row].Length; column++)
                    AssertCellMatches(expected, row, column);
            }
        }

        private void AssertCellMatches(bool[][] expected, int row, int column)
        {
            if (_board.CellIsAlive(row, column) != expected[row][column])
                ThrowMismatchedCellException(expected, row, column);
        }

        private void ThrowMismatchedCellException( bool[][] expected, int row, int column)
        {
            const string messageFormat = "Mismatch at Row {0}, Column {1}\r\nExpected: {2} but was: {3}";
            var message = string.Format(messageFormat, row, column, expected[row][column],
                                        _board.CellIsAlive(row, column));

            var actualRows = _board.GetBoard().Select(a => "| " + string.Join(" | ", a.Select(cell => cell ? "x" : ".").ToArray()) + " |");
            message += "\r\n" + string.Join("\r\n", actualRows.ToArray());

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