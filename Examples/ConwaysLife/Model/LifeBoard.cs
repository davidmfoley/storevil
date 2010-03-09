using System.Collections.Generic;
using System.Linq;

namespace ConwaysLife.Model
{
    /// <summary>
    /// This implements the basic rules to Conway's game of Life.
    /// (this is the system-under-test)
    /// </summary>
    public class LifeBoard
    {
        private bool[][] _table;

        public LifeBoard(bool[][] table)
        {
            _table = table;
        }

        public void Evolve()
        {          
            var newTable = new bool[RowCount][];

            for (int row = 0; row < RowCount; row++)
            {
                newTable[row] = new bool[ColumnCount];

                for (int col = 0; col < ColumnCount; col++)
                    newTable[row][col] = CalculateCell(row, col);
            }

            _table = newTable;
        }

        public bool CellIsAlive(int row, int col)
        {
            return !CellIsOutOfRange(row, col) && _table[row][col];
        }

        private bool CellIsOutOfRange(int row, int col)
        {
            return row < 0 || col < 0 || row >= RowCount || col >= ColumnCount;
        }

        public int RowCount
        {
            get { return _table.Length; }
        }

        public int ColumnCount
        {
            get { return _table[0].Length; }
        }

        private bool CalculateCell(int row, int col)
        {
            var aliveNeighborCount = GetNeighbors(row, col).Count(n => n);
            var cellIsAliveInCurrentGeneration = CellIsAlive(row, col);

            return ShouldLiveInNextGeneration(cellIsAliveInCurrentGeneration, aliveNeighborCount);
        }

        private static bool ShouldLiveInNextGeneration(bool cellIsAlive, int liveNeighbors)
        {
            if (cellIsAlive)
                return liveNeighbors == 2 || liveNeighbors == 3;

            return liveNeighbors == 3;
        }

        private IEnumerable<bool> GetNeighbors(int row, int col)
        {
            yield return CellIsAlive(row - 1, col - 1);
            yield return CellIsAlive(row - 1, col);
            yield return CellIsAlive(row - 1, col + 1);

            yield return CellIsAlive(row, col - 1);
            yield return CellIsAlive(row, col + 1);

            yield return CellIsAlive(row + 1, col - 1);
            yield return CellIsAlive(row + 1, col);
            yield return CellIsAlive(row + 1, col + 1);
        }

        public bool[][] GetBoard()
        {
            return _table;
        }
    }
}