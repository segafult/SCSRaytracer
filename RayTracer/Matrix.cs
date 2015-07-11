using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Class representing a 4 dimensional transformation matrix
    /// </summary>
    class Matrix
    {
        public double[,] tfVals;

        //Constructors
        public Matrix()
        {
            tfVals = new double[4,4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        }
    }
}
