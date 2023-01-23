using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame.Shapes
{
    internal class ZShape: Shape
    {
        public ZShape()
        {
            ArrayOfCells = new CellColor[][]
                {
                    new CellColor[3] { CellColor.Red, CellColor.Red, CellColor.Base },
                    new CellColor[3] { CellColor.Base, CellColor.Red, CellColor.Red },
                    new CellColor[3] { CellColor.Base, CellColor.Base, CellColor.Base },
                };
        }
    }
}
