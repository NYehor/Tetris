using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGame
{
    internal class JShape : Shape
    {
        public JShape() : base()
        { 
            ArrayOfCells = new CellColor[3][]
                {
                    new CellColor[3] { CellColor.Base, CellColor.Purple, CellColor.Base },
                    new CellColor[3] { CellColor.Base, CellColor.Purple, CellColor.Base },
                    new CellColor[3] { CellColor.Purple, CellColor.Purple, CellColor.Base },
                };
        }
    }
}
