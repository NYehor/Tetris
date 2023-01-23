using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    public abstract class Shape
    {
        public int PositionRow { get; set; } = 0;
        public int PositionCol { get; set; } = 3;
        public CellColor[][] ArrayOfCells { get; internal set; }

        public virtual void Rotate()
        {
            CellColor[][] newArrayOfCells = new CellColor[ArrayOfCells[0].Length][];

            for (int i = 0; i < ArrayOfCells[0].Length; i++)
                newArrayOfCells[i] = new CellColor[ArrayOfCells.Length];

            int maxLenIndx = ArrayOfCells.Length - 1;
            for (int i = 0; i < ArrayOfCells[0].Length; i++)
            {
                for (int j = 0; j < ArrayOfCells.Length; j++)
                {
                    newArrayOfCells[j][maxLenIndx - i] = ArrayOfCells[i][j];
                }
            }

            ArrayOfCells = newArrayOfCells;
        }

        public bool TryMoveLeft()
        {
            if (IsColumnEmpty(ArrayOfCells, 0))
            {
                for (int i = 0; i < ArrayOfCells.Length; i++)
                {
                    for (int j = 0; j < ArrayOfCells[0].Length - 1; j++)
                    { 
                        ArrayOfCells[i][j] = ArrayOfCells[i][j + 1];
                    }
                    ArrayOfCells[i][ArrayOfCells[i].Length - 1] = CellColor.Base;
                }

                return true;
            }

            return false;
        }

        public bool TryMoveRight()
        {
            if (IsColumnEmpty(ArrayOfCells, ArrayOfCells[0].Length - 1))
            {
                for (int i = 0; i < ArrayOfCells.Length; i++)
                {
                    for (int j = ArrayOfCells[0].Length - 1; j > 0; j--)
                    {
                        ArrayOfCells[i][j] = ArrayOfCells[i][j - 1];
                    }
                    ArrayOfCells[i][0] = CellColor.Base;
                }

                return true;
            }

            return false;
        }

        internal bool IsColumnEmpty(CellColor[][] matrix, int numberOfColumn)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                if (matrix[i][numberOfColumn] != CellColor.Base) return false;
            }

            return true;
        }

        public bool TryMoveDown()
        {
            bool IsRowEmpty = true;
            for (int i = 0; i < ArrayOfCells.Length; i++)
            {
                if (ArrayOfCells[ArrayOfCells.Length - 1][i] != CellColor.Base)
                    IsRowEmpty = false;
            }

            if (IsRowEmpty)
            {
                for (int i = 0; i < ArrayOfCells[0].Length; i++)
                {
                    for (int j = ArrayOfCells.Length - 1; j > 0; j--)
                        ArrayOfCells[j][i] = ArrayOfCells[j - 1][i];
                    ArrayOfCells[0][i] = CellColor.Base;
                }
            }

            return IsRowEmpty;
        }
    }
}
