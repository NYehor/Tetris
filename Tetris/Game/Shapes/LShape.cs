using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Game
{
    internal class LShape: Shape
    {
        public LShape()
        {
            ArrayOfCells = new CellColor[3][]
                {
                    new CellColor[3] { CellColor.Base, CellColor.Orange, CellColor.Base },
                    new CellColor[3] { CellColor.Base, CellColor.Orange, CellColor.Base },
                    new CellColor[3] { CellColor.Base, CellColor.Orange, CellColor.Orange },
                };
        }
    }
}
