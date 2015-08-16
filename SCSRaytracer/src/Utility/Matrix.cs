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
        public static double invThreeSixtyTwoPi = 1.0 / 360.0 * 2.0 * Math.PI;

        //Default constructor (identity matrix)
        public Matrix(int scale)
        {
            tfVals = new double[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            tfVals[0, 0] = scale;
            tfVals[1, 1] = scale;
            tfVals[2, 2] = scale;
            tfVals[3, 3] = scale;
        }

        ///----------------------------------------------------------------------------------------------------
        /// Static rotation related functions
        ///----------------------------------------------------------------------------------------------------
        public static Matrix inv_rotateX(double rot)
        {
            Matrix toReturn = new Matrix(1);
            //Set up transformation matrix
            toReturn.tfVals[1, 1] = Math.Cos(rot);
            toReturn.tfVals[2, 1] = -Math.Sin(rot);
            toReturn.tfVals[1, 2] = Math.Sin(rot);
            toReturn.tfVals[2, 2] = Math.Cos(rot);

            return toReturn;
        }
        public static Matrix inv_rotateXDeg(double rot)
        {
            return inv_rotateX(rot * invThreeSixtyTwoPi);
        }
        public static Matrix inv_rotateY(double rot)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = Math.Cos(rot);
            toReturn.tfVals[2, 0] = Math.Sin(rot);
            toReturn.tfVals[0, 2] = -Math.Sin(rot);
            toReturn.tfVals[2, 2] = Math.Cos(rot);

            return toReturn;
        }
        public static Matrix inv_rotateYDeg(double rot)
        {
            return inv_rotateY(rot * invThreeSixtyTwoPi);
        }
        public static Matrix inv_rotateZ(double rot)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = Math.Cos(rot);
            toReturn.tfVals[1, 0] = -Math.Sin(rot);
            toReturn.tfVals[0, 1] = Math.Sin(rot);
            toReturn.tfVals[1, 1] = Math.Cos(rot);

            return toReturn;
        }
        public static Matrix inv_rotateZDeg(double rot)
        {
            return inv_rotateZ(rot * invThreeSixtyTwoPi);
        }
        public static Matrix inv_rotateDeg(Vect3D rotations)
        {
            return inv_rotateXDeg(rotations.xcoord) * inv_rotateYDeg(rotations.ycoord) * inv_rotateZDeg(rotations.zcoord);
        }
        public static Matrix rotateX(double rot)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[1, 1] = Math.Cos(rot);
            toReturn.tfVals[2, 1] = Math.Sin(rot);
            toReturn.tfVals[1, 2] = -Math.Sin(rot);
            toReturn.tfVals[2, 2] = Math.Cos(rot);
            return toReturn;
        }
        public static Matrix rotateXDeg(double rot)
        {
            return rotateX(rot * invThreeSixtyTwoPi);
        }
        public static Matrix rotateY(double rot)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = Math.Cos(rot);
            toReturn.tfVals[2, 0] = -Math.Sin(rot);
            toReturn.tfVals[0, 2] = Math.Sin(rot);
            toReturn.tfVals[2, 2] = Math.Cos(rot);
            return toReturn;
        }
        public static Matrix rotateYDeg(double rot)
        {
            return rotateY(rot * invThreeSixtyTwoPi);
        }
        public static Matrix rotateZ(double rot)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = Math.Cos(rot);
            toReturn.tfVals[1, 0] = Math.Sin(rot);
            toReturn.tfVals[1, 0] = -Math.Sin(rot);
            toReturn.tfVals[1, 1] = Math.Cos(rot);
            return toReturn;
        }
        public static Matrix rotateZDeg(double rot)
        {
            return rotateZ(rot * invThreeSixtyTwoPi);
        }
        public static Matrix rotateDeg(Vect3D rotations)
        {
            return rotateZDeg(rotations.zcoord) * rotateYDeg(rotations.ycoord) * rotateXDeg(rotations.xcoord);
        }

        ///----------------------------------------------------------------------------------------------------
        /// Static scaling related functions
        ///----------------------------------------------------------------------------------------------------
        public static Matrix inv_scale(Vect3D scaleFactor)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = 1 / scaleFactor.xcoord;
            toReturn.tfVals[1, 1] = 1 / scaleFactor.ycoord;
            toReturn.tfVals[2, 2] = 1 / scaleFactor.zcoord;

            return toReturn;
        }
        public static Matrix inv_scale(double x, double y, double z)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = 1 / x;
            toReturn.tfVals[1, 1] = 1 / y;
            toReturn.tfVals[2, 2] = 1 / z;

            return toReturn;
        }
        public static Matrix scale(Vect3D scalefactor)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = scalefactor.xcoord;
            toReturn.tfVals[1, 1] = scalefactor.ycoord;
            toReturn.tfVals[2, 2] = scalefactor.zcoord;
            return toReturn;
        }
        public static Matrix scale(double x, double y, double z)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 0] = x;
            toReturn.tfVals[1, 1] = y;
            toReturn.tfVals[2, 2] = z;
            return toReturn;
        }

        ///----------------------------------------------------------------------------------------------------
        /// Static translation related functions
        ///----------------------------------------------------------------------------------------------------
        public static Matrix inv_translate(Vect3D translation)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 3] = -translation.xcoord;
            toReturn.tfVals[1, 3] = -translation.ycoord;
            toReturn.tfVals[2, 3] = -translation.zcoord;

            return toReturn;
        }
        public static Matrix inv_translate(double x, double y, double z)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 3] = -x;
            toReturn.tfVals[1, 3] = -y;
            toReturn.tfVals[2, 3] = -z;

            return toReturn;
        }
        public static Matrix translate(Vect3D translation)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 3] = translation.xcoord;
            toReturn.tfVals[1, 3] = translation.ycoord;
            toReturn.tfVals[2, 3] = translation.zcoord;
            return toReturn;
        }
        public static Matrix translate(double x, double y, double z)
        {
            Matrix toReturn = new Matrix(1);
            toReturn.tfVals[0, 3] = x;
            toReturn.tfVals[1, 3] = y;
            toReturn.tfVals[2, 3] = z;
            return toReturn;
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
        //Applying a transformation matrix to a normal (unaffected by translation, rows/columns of matrix flipped)
        public static Normal operator *(Matrix mat, Normal n)
        {
            return new Normal(mat.tfVals[0, 0] * n.xcoord + mat.tfVals[1, 0] * n.ycoord + mat.tfVals[2, 0] * n.zcoord,
                mat.tfVals[0, 1] * n.xcoord + mat.tfVals[1, 1] * n.ycoord + mat.tfVals[2, 1] * n.zcoord,
                mat.tfVals[0, 2] * n.xcoord + mat.tfVals[1, 2] * n.ycoord + mat.tfVals[2, 2] * n.zcoord);
        }
        //Applying a transformation matrix to a point (affected by translation)
        public static Point3D operator *(Matrix mat, Point3D p)
        {
            return new Point3D((mat.tfVals[0,0] * p.xcoord) + (mat.tfVals[0,1] * p.ycoord) + (mat.tfVals[0,2]*p.zcoord) + mat.tfVals[0,3],
                (mat.tfVals[1, 0] * p.xcoord) + (mat.tfVals[1, 1] * p.ycoord) + (mat.tfVals[1, 2] * p.zcoord) + mat.tfVals[1, 3],
                (mat.tfVals[2, 0] * p.xcoord) + (mat.tfVals[2, 1] * p.ycoord) + (mat.tfVals[2, 2] * p.zcoord) + mat.tfVals[2, 3]
                );
        }

        //4x4 Matrix multiplication
        public static Matrix operator *(Matrix l, Matrix r)
        {
            Matrix result = new Matrix(1);
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
