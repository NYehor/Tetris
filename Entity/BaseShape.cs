using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class BaseShape : IShape
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public Matrix3x3 Matrix { get; set; }

        internal Cell[] Shape;

        public BaseShape(Matrix3x3 matrix, Cell[] cells, int width, int height)
        { 
            Matrix = matrix;
            Shape = cells;
            Width = width;
            Height = height;
        }

        public Cell[] GetCells()
        {
            Cell[] cells = new Cell[Shape.Length];
            Array.Copy(Shape, cells, Shape.Length);

            return cells;
        }

        public BaseShape SetCells(Cell[] cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException("cells");
            }

            Shape = cells;
            return this;
        }

        public void SwapWidthAndHeight()
        {
            var tmp = Height;
            Height = Width;
            Width = tmp;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
