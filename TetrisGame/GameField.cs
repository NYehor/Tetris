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
        public IShape ActiveElemnt {get; internal set;}

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

        public void SetNextElement()
        {

            var cells = ActiveElemnt.GetCells();
            
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Location = ActiveElemnt.Matrix * cells[i].Location;
            }

            Oddments.AddRange(cells);
            Score += Width * CountOfRemovedCompletedRows();
            ActiveElemnt = shapeFactory.GenerateShape(new Vector3((short)(Width / 2 - 1), 0, 0));
        }

        public bool TrySetNextElement()
        { 
            bool result = true;
            SetNextElement();

            var cells = ActiveElemnt.GetCells();
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Location = ActiveElemnt.Matrix * cells[i].Location;
                if (IsExistCellInOddments(cells[i]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public bool IsExistCellInOddments(Cell cell)
        {
            return Oddments.Exists(c => c.Row == cell.Row && c.Column == cell.Column);
        }

        public int CountOfRemovedCompletedRows()
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
