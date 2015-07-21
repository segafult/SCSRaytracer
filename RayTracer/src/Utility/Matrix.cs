//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Class representing a 4 dimensional transformation matrix
    /// </summary>
    class Matrix
    {
        public double[,] tfVals;

        //Default constructor (identity matrix)
        public Matrix()
        {
            tfVals = new double[4,4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            tfVals[0, 0] = 1;
            tfVals[1, 1] = 1;
            tfVals[2, 2] = 1;
            tfVals[3, 3] = 1;
        }

        //Operator overloads
        //Applying a transformation matrix to a vector (unaffected by translation)
        public static Vect3D operator *(Matrix mat, Vect3D v)
        {
            return new Vect3D(mat.tfVals[0, 0] * v.xcoord + mat.tfVals[0, 1] * v.ycoord + mat.tfVals[0, 2] * v.zcoord,
                mat.tfVals[1, 0] * v.xcoord + mat.tfVals[1, 1] * v.ycoord + mat.tfVals[1, 2] * v.zcoord,
                mat.tfVals[2, 0] * v.xcoord + mat.tfVals[2, 1] * v.ycoord + mat.tfVals[2, 2] * v.zcoord
                );
        }
        //Applying a transformation matrix to a point (affected by translation)
        public static Point3D operator *(Matrix mat, Point3D p)
        {
            return new Point3D((mat.tfVals[0,0] * p.xcoord) + (mat.tfVals[0,1] * p.ycoord) + (mat.tfVals[0,2]*p.zcoord) + mat.tfVals[0,3],
                (mat.tfVals[1, 0] * p.xcoord) + (mat.tfVals[1, 1] * p.ycoord) + (mat.tfVals[1, 2] * p.zcoord) + mat.tfVals[1, 3],
                (mat.tfVals[2, 0] * p.xcoord) + (mat.tfVals[2, 1] * p.ycoord) + (mat.tfVals[2, 2] * p.zcoord) + mat.tfVals[2, 3]
                );
        }
        //Applying a transformation matrix to a normal

        //4x4 Matrix multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix operator *(Matrix l, Matrix r)
        {
            Matrix result = new Matrix();
            //First row
            result.tfVals[0, 0] =   (l.tfVals[0, 0] * r.tfVals[0, 0]) +
                                    (l.tfVals[0, 1] * r.tfVals[1, 0]) +
                                    (l.tfVals[0, 2] * r.tfVals[2, 0]) +
                                    (l.tfVals[0, 3] * r.tfVals[3, 0]);
            result.tfVals[0, 1] =   (l.tfVals[0, 0] * r.tfVals[0, 1]) +
                                    (l.tfVals[0, 1] * r.tfVals[1, 1]) +
                                    (l.tfVals[0, 2] * r.tfVals[2, 1]) +
                                    (l.tfVals[0, 3] * r.tfVals[3, 1]);
            result.tfVals[0, 2] =   (l.tfVals[0, 0] * r.tfVals[0, 2]) +
                                    (l.tfVals[0, 1] * r.tfVals[1, 2]) +
                                    (l.tfVals[0, 2] * r.tfVals[2, 2]) +
                                    (l.tfVals[0, 3] * r.tfVals[3, 2]);
            result.tfVals[0, 3] =   (l.tfVals[0, 0] * r.tfVals[0, 3]) +
                                    (l.tfVals[0, 1] * r.tfVals[1, 3]) +
                                    (l.tfVals[0, 2] * r.tfVals[2, 3]) +
                                    (l.tfVals[0, 3] * r.tfVals[3, 3]);

            //Second row
            result.tfVals[1, 0] =   (l.tfVals[1, 0] * r.tfVals[0, 0]) +
                                    (l.tfVals[1, 1] * r.tfVals[1, 0]) +
                                    (l.tfVals[1, 2] * r.tfVals[2, 0]) +
                                    (l.tfVals[1, 3] * r.tfVals[3, 0]);
            result.tfVals[1, 1] =   (l.tfVals[1, 0] * r.tfVals[0, 1]) +
                                    (l.tfVals[1, 1] * r.tfVals[1, 1]) +
                                    (l.tfVals[1, 2] * r.tfVals[2, 1]) +
                                    (l.tfVals[1, 3] * r.tfVals[3, 1]);
            result.tfVals[1, 2] =   (l.tfVals[1, 0] * r.tfVals[0, 2]) +
                                    (l.tfVals[1, 1] * r.tfVals[1, 2]) +
                                    (l.tfVals[1, 2] * r.tfVals[2, 2]) +
                                    (l.tfVals[1, 3] * r.tfVals[3, 2]);
            result.tfVals[1, 3] =   (l.tfVals[1, 0] * r.tfVals[0, 3]) +
                                    (l.tfVals[1, 1] * r.tfVals[1, 3]) +
                                    (l.tfVals[1, 2] * r.tfVals[2, 3]) +
                                    (l.tfVals[1, 3] * r.tfVals[3, 3]);

            //Third row
            result.tfVals[2, 0] =   (l.tfVals[2, 0] * r.tfVals[0, 0]) +
                                    (l.tfVals[2, 1] * r.tfVals[1, 0]) +
                                    (l.tfVals[2, 2] * r.tfVals[2, 0]) +
                                    (l.tfVals[2, 3] * r.tfVals[3, 0]);
            result.tfVals[2, 1] =   (l.tfVals[2, 0] * r.tfVals[0, 1]) +
                                    (l.tfVals[2, 1] * r.tfVals[1, 1]) +
                                    (l.tfVals[2, 2] * r.tfVals[2, 1]) +
                                    (l.tfVals[2, 3] * r.tfVals[3, 1]);
            result.tfVals[2, 2] =   (l.tfVals[2, 0] * r.tfVals[0, 2]) +
                                    (l.tfVals[2, 1] * r.tfVals[1, 2]) +
                                    (l.tfVals[2, 2] * r.tfVals[2, 2]) +
                                    (l.tfVals[2, 3] * r.tfVals[3, 2]);
            result.tfVals[2, 3] =   (l.tfVals[2, 0] * r.tfVals[0, 3]) +
                                    (l.tfVals[2, 1] * r.tfVals[1, 3]) +
                                    (l.tfVals[2, 2] * r.tfVals[2, 3]) +
                                    (l.tfVals[2, 3] * r.tfVals[3, 3]);

            //Fourth row
            result.tfVals[3, 0] =   (l.tfVals[3, 0] * r.tfVals[0, 0]) +
                                    (l.tfVals[3, 1] * r.tfVals[1, 0]) +
                                    (l.tfVals[3, 2] * r.tfVals[2, 0]) +
                                    (l.tfVals[3, 3] * r.tfVals[3, 0]);
            result.tfVals[3, 1] =   (l.tfVals[3, 0] * r.tfVals[0, 1]) +
                                    (l.tfVals[3, 1] * r.tfVals[1, 1]) +
                                    (l.tfVals[3, 2] * r.tfVals[2, 1]) +
                                    (l.tfVals[3, 3] * r.tfVals[3, 1]);
            result.tfVals[3, 2] =   (l.tfVals[3, 0] * r.tfVals[0, 2]) +
                                    (l.tfVals[3, 1] * r.tfVals[1, 2]) +
                                    (l.tfVals[3, 2] * r.tfVals[2, 2]) +
                                    (l.tfVals[3, 3] * r.tfVals[3, 2]);
            result.tfVals[3, 3] =   (l.tfVals[3, 0] * r.tfVals[0, 3]) +
                                    (l.tfVals[3, 1] * r.tfVals[1, 3]) +
                                    (l.tfVals[3, 2] * r.tfVals[2, 3]) +
                                    (l.tfVals[3, 3] * r.tfVals[3, 3]);
            return result;
        }
    }
}
