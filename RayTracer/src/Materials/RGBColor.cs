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
using System.Drawing;

namespace RayTracer
{
    public class RGBColor
    {
        public double r;
        public double g;
        public double b;

        public override string ToString()
        {
            return "#" + Convert.ToByte(r * 255.0).ToString("x") +
                Convert.ToByte(g * 255.0).ToString("x") + 
                Convert.ToByte(b * 255.0).ToString("x");
        }
        public RGBColor()
        {
            //Default color = grey
            r = 0.5;
            g = 0.5;
            b = 0.5;
        }
        public RGBColor (double red, double green, double blue)
        {
            r = red;
            g = green;
            b = blue;
        }
        public RGBColor (System.Drawing.Color col)
        {
            r = (double)col.R / 255.0;
            g = (double)col.G / 255.0;
            b = (double)col.B / 255.0;
        }
        //Copy constructor
        public RGBColor (RGBColor color)
        {
            //No need to clamp when cloning, can assume values are safe.
            r = color.r;
            g = color.g;
            b = color.b;
        }

        //Setters. No getter functions, members public for performance reasons. :)
        public void setRed(double r)
        {
            this.r = r;
        }
        public void setGreen(double g)
        {
            this.g = g;
        }
        public void setBlue(double b)
        {
            this.b = b;
        }
        /*
        /// <summary>
        /// Returns a color that is effectively the sum of two other colors.
        /// </summary>
        /// <param name="c2">Color to add</param>
        /// <returns>Sum of this and c2</returns>
        public RGBColor addColor(RGBColor c2)
        {
            return new RGBColor(this.r + c2.r, this.g + c2.g, this.b + c2.b);
        }

        /// <summary>
        /// Scales color by a given scalar value.
        /// </summary>
        /// <param name="s">Scalar value to multiply color by</param>
        /// <returns>New RGBColor with scaled rgb components</returns>
        public RGBColor multiplyScalar(double s)
        {
            return new RGBColor(this.r * s, this.g * s, this.b * s);
        }

        /// <summary>
        /// Multiplies two rgb colors together
        /// </summary>
        /// <param name="c2">Color to multiply this color by</param>
        /// <returns>New product rgb color</returns>
        public RGBColor multiplyColor(RGBColor c2)
        {
            return new RGBColor(this.r * c2.r, this.g * c2.g, this.b * c2.b);
        }

        /// <summary>
        /// Raises a color to a given power
        /// </summary>
        /// <param name="p">Power to raise given color to</param>
        /// <returns>New RGB color raised to a given power</returns>
        public RGBColor pow(double p)
        {
            return new RGBColor(Math.Pow(this.r, p), Math.Pow(this.g, p), Math.Pow(this.b, p));
        }
        */
        public RGBColor clamp()
        {
            double r_c, g_c, b_c;
            r_c = clamp_to_range(r, 0.0, 1.0);
            g_c = clamp_to_range(g, 0.0, 1.0);
            b_c = clamp_to_range(b, 0.0, 1.0);
            return new RGBColor(r_c, g_c, b_c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double clamp_to_range(double value, double lowerbound, double upperbound)
        {
            if(value < lowerbound) { return lowerbound; }
            else if(value > upperbound) { return upperbound; }
            else { return value; }
        }

        ///
        ///Operator overloads
        ///
        //Color addition
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator +(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b);
        }

        //Scalar multiplication of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(double a, RGBColor c)
        {
            return new RGBColor(c.r * a, c.g * a, c.b * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c, double a)
        {
            return new RGBColor(c.r * a, c.g * a, c.b * a);
        }

        //Scalar division of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator /(RGBColor c, double a)
        {
            double inva = 1 / a;
            return new RGBColor(c.r * inva, c.g * inva, c.b * inva);
        }

        //Multiplication of color with a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b);
        }

        //Raising color to a power
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator ^(RGBColor c, double p)
        {
            return new RGBColor(Math.Pow(c.r, p), Math.Pow(c.g, p), Math.Pow(c.b, p));
        }
    }
}
