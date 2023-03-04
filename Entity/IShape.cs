using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public interface IShape: ICloneable
    {
        Matrix3x3 Matrix { get; set; }
        int Width { get; }
        int Height { get; }

        Cell[] GetCells();
        void SwapWidthAndHeight();
    }
}
