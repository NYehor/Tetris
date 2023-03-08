using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Enumeration;
using System.Drawing;
using Entity;
using Data;

namespace TetrisGame
{
    public class ShapeFactory
    {
        private Random random;
        private List<IShape> shapes; 

        public ShapeFactory(Profile profile) 
        {
            random = new Random();
            shapes = new List<IShape>();
            shapes = profile.Elements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public IShape GenerateShape(Vector3 location)
        {
            var shape = (IShape)shapes[random.Next(0, shapes.Count())].Clone();
            shape.Matrix = Matrix3x3.Translate(shape.Matrix, location);

            return shape;
        }

        public IShape AddShape(CellColor[,] cells, CellColor cellColor)
        {
            if (cells == null)
            {
                throw new ArgumentNullException("cells");
            }

            int width = cells.GetUpperBound(0) + 1;
            int height = cells.GetUpperBound(1) + 1;

            List<Cell> shapeCells = new List<Cell>();
            int w = 0;
            int h = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (cells[i, j] != CellColor.Base)
                    {
                        shapeCells.Add(new Cell((short)i, (short)j, cellColor));

                        if (w < i)
                        { 
                            w = i;
                        }

                        if (h < j) 
                        {
                            h = j;
                        }
                    }
                }
            }

            var s = new BaseShape(new Matrix3x3(), shapeCells.ToArray(), w + 1, h + 1);
            shapes.Add(s);

            return s;
        }
    }
}
