using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Entity
{
    public struct Cell
    {
        private Vector3 point;

        public Vector3 Location
        {
            get => point;
            set => point = value;
        }

        public short Row 
        { 
            get => (short)point[1]; 
            set => point[1] = value;
        }

        public short Column 
        { 
            get => (short)point[0];
            set => point[0] = value;
        }

        public CellColor Color { get; set; }

        public Cell(short row, short column, CellColor color)
        {
            Color = color;
            point = new Vector3(column, row, 1);
        }
    }
}
