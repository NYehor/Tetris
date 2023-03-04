using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public struct Matrix3x3
    {
        private short[,] matrix;
        public int this[short r, short c]
        {
            get => matrix[r, c];
            set => matrix[r, c] = (short)value;
        }

        public Matrix3x3(Matrix3x3 matrix)
        {
            this.matrix = new short[3, 3];
            for (short i = 0; i < 3; i++)
            {
                for (short j = 0; j < 3; j++)
                {
                    this.matrix[i, j] = (short)matrix[i, j];
                }
            }
        }

        public Matrix3x3()
        {
            matrix = new short[3, 3];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
        }

        public Matrix3x3(
            short m11, short m12, short m13, 
            short m21, short m22, short m23,
            short m31, short m32, short m33
            )
        {
            matrix = new short[3, 3];
            matrix[0, 0] = m11;
            matrix[0, 1] = m12;
            matrix[0, 2] = m13;
            matrix[1, 0] = m21;
            matrix[1, 1] = m22;
            matrix[1, 2] = m23;
            matrix[2, 0] = m31;
            matrix[2, 1] = m32;
            matrix[2, 2] = m33;       
        }

        public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
        { 
            Matrix3x3 m = new Matrix3x3();

            for (short i = 0; i < 3; i++)
            {
                for (short j = 0; j < 3; j++)
                {
                    m[i, j] = 0;
                    for (short c = 0; c < 3; c++)
                    {
                        m[i, j] += m1[i, c] * m2[c, j];
                    }
                }
            }

            return m;
        }

        public static Matrix3x3 Translate(Matrix3x3 matrix, Vector3 vector)
        {
            var newMatrix = new Matrix3x3(matrix);

            newMatrix[0, 2] += vector[0];
            newMatrix[1, 2] += vector[1];

            return newMatrix;
        }

        public static Matrix3x3 Rotate(double angle, Vector3 point)
        {
            short c = (short)Math.Cos(angle * Math.PI / 180.0);
            short s = (short)Math.Sin(angle * Math.PI / 180.0);

            Matrix3x3 r = new Matrix3x3(c, (short)-s, 0, s, c, 0, 0, 0, 1);
            
            var t = Translate(new Matrix3x3(), point);
            var t1 = Translate(new Matrix3x3(), new Vector3((short)-point[0], (short)-point[1], (short)point[2]));

            return t * r * t1;
        }
    }
}
