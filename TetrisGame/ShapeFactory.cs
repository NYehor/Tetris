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
        private Profile profile;

        public ShapeFactory(ref Profile profile) 
        {
            random = new Random();
            this.profile = profile;
        }

        /// <summary>
        ///     Method return the next random shape
        /// </summary>
        /// <param name="location"></param>
        /// <returns>New shape</returns>
        public IShape GenerateShape(Vector3 location)
        {
            if (profile.Elements.Count == 0)
            {
                throw new InvalidOperationException("ShapeFactory can not generate an element, because the list of elements is empty.");
            }

            IShape shape = (IShape)profile.Elements[random.Next(0, profile.Elements.Count())].Clone();
            shape.Matrix = Matrix3x3.Translate(shape.Matrix, location);

            Angle angle = (Angle)random.Next(0, 3);
            shape.Matrix = shape.Matrix * Matrix3x3.Rotate(angle, new Vector3(1, 1, 1));

            if (angle == Angle.Turn90 || angle == Angle.Turn270)
            {
                shape.SwapWidthAndHeight();
            }

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
            bool isEmpty = true;
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

                        isEmpty = false;
                    }
                }
            }

            var s = new BaseShape(new Matrix3x3(), shapeCells.ToArray(), w + 1, h + 1);
            if (!isEmpty)
            {
                profile.Elements.Add(s);
            }

            return s;
        }
    }
}
