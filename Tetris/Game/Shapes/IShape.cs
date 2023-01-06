using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Game
{
    internal class IShape: Shape
    {
        public IShape()
        {
            ArrayOfCells = new CellColor[4][]
                {
                    new CellColor[4] { CellColor.Base, CellColor.Green, CellColor.Base, CellColor.Base },
                    new CellColor[4] { CellColor.Base, CellColor.Green, CellColor.Base, CellColor.Base },
                    new CellColor[4] { CellColor.Base, CellColor.Green, CellColor.Base, CellColor.Base },
                    new CellColor[4] { CellColor.Base, CellColor.Green, CellColor.Base, CellColor.Base },
                };
        }
    }
}
