using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace TetrisGame
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public int Score { get; internal set; }

        private ShapeFactory shapeFactory;
        public IShape ActiveElemnt { get; internal set; }

        private List<Cell> Oddments;
        private CellColor[,] Frame;

        public GameField(int width, int height, CellColor[,] frame, ShapeFactory shapeFactory)
        {
            Width = width;
            Height = height;
            Frame = frame;
            Score = 0;
            this.shapeFactory = shapeFactory;
            ActiveElemnt = this.shapeFactory.GenerateShape(new Vector3((short)(Width / 2 - 1), 0, 0));
            Oddments = new List<Cell>();
        }


        /// <summary>
        ///     Try to add next element
        /// </summary>
        /// <returns>True if the next element added</returns>
        public bool TryAddNextElement()
        {
            bool result = true;

            var newOddments = ActiveElemnt.GetCells();
            for (int i = 0; i < newOddments.Length; i++)
            {
                newOddments[i].Location = ActiveElemnt.Matrix * newOddments[i].Location;
            }

            Oddments.AddRange(newOddments);
            Score += Width * CountOfRemovedCompletedRows();

            var newActiveElemnt = shapeFactory.GenerateShape(new Vector3((short)(Width / 2 - 1), 0, 0));
            var cells = newActiveElemnt.GetCells();
            for (int i = 0; i < cells.Length && result; i++)
            {
                cells[i].Location = newActiveElemnt.Matrix * cells[i].Location;
                result = !IsExistCellInOddments(cells[i]);
            }

            if (result)
            {
                ActiveElemnt = newActiveElemnt;
            }

            return result;
        }

        /// <summary>
        ///    The method checks if the cell exists in oddments
        /// </summary>
        /// <param name="cell">Cell for check</param>
        /// <returns>True if the cell exists in oddments</returns>
        public bool IsExistCellInOddments(Cell cell)
        {
            return Oddments.Exists(c => c.Row == cell.Row && c.Column == cell.Column);
        }

        private int CountOfRemovedCompletedRows()
        {
            int count = 0;

            for (int i = 0; i < Height; i++)
            {
                if (Oddments.Where(c => c.Row == i).Count() == Width)
                {
                    Oddments.RemoveAll(c => c.Row == i);
                    Oddments.ForEach(c =>
                    {
                        if (c.Row < i)
                        {
                            c.Row++;
                        }
                    });

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// The frame setup 
        /// </summary>
        public void DrawGameField()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Frame[i, j] = CellColor.Base;
                }
            }

            foreach (var cell in Oddments)
            {
                Frame[cell.Column, cell.Row] = cell.Color;
            }

            var cells = ActiveElemnt.GetCells();
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Location = ActiveElemnt.Matrix * cells[i].Location;
                Frame[cells[i].Column, cells[i].Row] = cells[i].Color;
            }
        }
    }
}
