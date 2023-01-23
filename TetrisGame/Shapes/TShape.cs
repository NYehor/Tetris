using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class TShape: Shape
    {
        public TShape()
        {
            ArrayOfCells = new CellColor[3][]
                {
                    new CellColor[3] { CellColor.Base, CellColor.Blue, CellColor.Base },
                    new CellColor[3] { CellColor.Blue, CellColor.Blue, CellColor.Blue },
                    new CellColor[3] { CellColor.Base, CellColor.Base, CellColor.Base },
                };
        }
    }
}
