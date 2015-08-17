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
using System.Runtime.CompilerServices;
using System.Numerics;

namespace RayTracer
{
    public struct RGBColor
    {
        public static Vector3 BLACK = new Vector3(0.0f, 0.0f, 0.0f);
        public static Vector3 WHITE = new Vector3(255.0f, 255.0f, 255.0f);
        public Vector3 vals;

        public override string ToString()
        {
            return "#" + Convert.ToByte(vals.X * 255.0).ToString("x") +
                Convert.ToByte(vals.Y * 255.0).ToString("x") + 
                Convert.ToByte(vals.Z * 255.0).ToString("x");
        }

        public RGBColor (float red, float green, float blue)
        {
            vals = new Vector3(red, green, blue);
        }
        public RGBColor (Vector3 v)
        {
            vals = v;
        }
        public RGBColor (System.Drawing.Color col)
        {
            vals = new Vector3(col.R * FastMath.INVTWOFITTYFI, col.G * FastMath.INVTWOFITTYFI, col.B * FastMath.INVTWOFITTYFI);
        }
        public RGBColor (SFML.Graphics.Color col)
        {
            vals = new Vector3(col.R * FastMath.INVTWOFITTYFI, col.G * FastMath.INVTWOFITTYFI, col.B * FastMath.INVTWOFITTYFI);
        }
        //Copy constructor
        public RGBColor (RGBColor color)
        {
            vals = color.vals;
        }  
        
        public RGBColor clamp()
        {
            return new RGBColor(Vector3.Clamp(vals, BLACK, WHITE));
        }

        ///
        ///Operator overloads
        ///
        //Color addition
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator +(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1.vals + c2.vals);
        }

        //Scalar multiplication of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(float a, RGBColor c)
        {
            return new RGBColor(c.vals * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c, float a)
        {
            return new RGBColor(c.vals * a);
        }

        //Scalar division of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator /(RGBColor c, float a)
        {
            return new RGBColor(c.vals / a);
        }

        //Multiplication of color with a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1.vals * c2.vals);
        }

        //Raising color to a power
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator ^(RGBColor c, float p)
        {
            return new RGBColor((float)Math.Pow(c.vals.X, p), (float)Math.Pow(c.vals.Y, p), (float)Math.Pow(c.vals.Z, p));
        }
    }
}
