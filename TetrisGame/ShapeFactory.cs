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
            shapes = profile.Elements;
        }

        /// <summary>
        ///     Method return the next random shape
        /// </summary>
        /// <param name="location"></param>
        /// <returns>New shape</returns>
        public IShape GenerateShape(Vector3 location)
        {
            if (shapes.Count == 0)
            {
                throw new InvalidOperationException("ShapeFactory can not generate an element, because the list of elements is empty.");
            }

            IShape shape = (IShape)shapes[random.Next(0, shapes.Count())].Clone();
            shape.Matrix = Matrix3x3.Translate(shape.Matrix, location);

            return shape;
        }

        /// <summary>
        ///     Add the new element to the list
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="cellColor"></param>
        /// <returns>The new element</returns>
        /// <exception cref="ArgumentNullException"></exception>
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
                        shapeCells.Add(new Cell((short)j, (short)i, cellColor));

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
