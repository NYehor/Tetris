using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Enumeration;
using System.Drawing;
using Entity;

namespace TetrisGame
{
    public class ShapeFactory
    {
        private Random random;
        private List<IShape> shapes; 
        private readonly string FileName;

        public ShapeFactory() 
        {
            FileName = "TetrisShapes.txt";
            random = new Random();
            shapes = ReadFile();
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

        public void WriteFile(IShape shape)
        {
            string path = Environment.CurrentDirectory;
            string fullPath = Path.Combine(path, FileName);

            string text = "Matrix:{";
            for (short i = 0; i < 3; i++)
            { 
                for (short j = 0; j < 3 && !(j == 2 && i == 2); j++)
                {
                    text += shape.Matrix[i, j] + ",";
                }
            }

            text += shape.Matrix[2, 2].ToString();
            text += "}";

            text += " Cells:{";
            var cells = shape.GetCells();

            foreach (var cell in cells)
            {
                text += "[Row:" + cell.Row.ToString() + " Column:" + cell.Column.ToString() + " Color:" + cell.Color + "] ";
            }

            text += "} ";
            text += "Width:{" + shape.Width + "} ";
            text += "Height:{" + shape.Height + "}";

            using StreamWriter file = new (fullPath, append: true);
            file.WriteLine(text);
        }

        private List<IShape> ReadFile() 
        {
            string path = Environment.CurrentDirectory;
            string fullPath = Path.Combine(path, FileName);
            var shapes = new List<IShape>();

            string[] lines = File.ReadAllLines(fullPath);

            foreach (string line in lines)
            {                
                int sMatrixIndx = line.IndexOf("Matrix:{") + 8;
                int fMatrixIndx = line.IndexOf("}", sMatrixIndx);

                string matrixStr = line.Substring(sMatrixIndx, fMatrixIndx - sMatrixIndx);
                var numbStr = matrixStr.Split(',');

                var matrix = new Matrix3x3();
                int numbIndex = 0;
                for (short i = 0; i < 3; i++)
                {
                    for (short j = 0; j < 3; j++)
                    {
                        matrix[i, j] = Convert.ToInt32(numbStr[numbIndex]);
                        numbIndex++;
                    }
                }

                int sCellsIndex = line.IndexOf("Cells:{[") + 8;
                int fCellsIndex = line.IndexOf("] }", sCellsIndex);
                string[] cellsStr = line.Substring(sCellsIndex, fCellsIndex - sCellsIndex).Split("] [");

                Cell[] cells = new Cell[cellsStr.Length];
                for (int i = 0; i < cellsStr.Length; i++)
                {
                    int rowInd = cellsStr[i].IndexOf("Row:") + 4;
                    int columnInd = cellsStr[i].IndexOf("Column:") + 7;
                    int colorInd = cellsStr[i].IndexOf("Color:") + 6;

                    short row = Convert.ToInt16(cellsStr[i].Substring(rowInd, columnInd - rowInd - 7).Trim(' '));
                    short column = Convert.ToInt16(cellsStr[i].Substring(columnInd, colorInd - columnInd - 6).Trim(' '));
                    string colorStr = cellsStr[i].Substring(colorInd, cellsStr[i].Length - colorInd).Trim(' ');

                    cells[i] = new Cell(row, column, (CellColor)Enum.Parse(typeof(CellColor), colorStr));
                }

                int widthInd = line.IndexOf("Width:{") + 7;
                string widthStr = line.Substring(widthInd, line.IndexOf("}", widthInd) - widthInd).Trim(' ');
                int width = Convert.ToInt32(widthStr);

                int heightInd = line.IndexOf("Height:{") + 8;
                string heightStr = line.Substring(heightInd, line.IndexOf("}", heightInd) - heightInd).Trim(' ');
                int height = Convert.ToInt32(heightStr);

                shapes.Add(new BaseShape(matrix, cells, width, height));
            }

            return shapes;
        }
    }
}
