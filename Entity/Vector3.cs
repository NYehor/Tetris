using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public struct Vector3
    {
        private short[] array;

        public int this[short i]
        {
            get => array[i];
            set => array[i] = (short)value;
        }

        public Vector3(short x, short y, short z)
        {
            array = new short[3];
            array[0] = x;
            array[1] = y;
            array[2] = z;
        }

        /// <summary>
        ///     The operator to multiply matrix on vector
        /// </summary>
        /// <param name="matrix"> Matrix </param>
        /// <param name="vector"> Vector </param>
        /// <returns>The result of multyply operator</returns>
        public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
        { 
            Vector3 v = new Vector3(0,0,0);
            for (short i = 0; i < 3; i++)
            {
                for (short j = 0; j < 3; j++)
                {
                    v[i] += matrix[i, j] * vector[j];
                }
            }

            return v;
        }

        public static Vector3 operator -(Vector3 vector)
        {
            Vector3 v = new Vector3(0, 0, 0);
            for (short j = 0; j < 3; j++)
            {
                v[j] = - vector[j];
            }
            
            return v;
        }
    }
}
