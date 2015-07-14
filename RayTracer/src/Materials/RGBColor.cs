using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class RGBColor
    {
        public double r;
        public double g;
        public double b;

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

        ///
        ///Operator overloads
        ///
        public static RGBColor operator +(RGBColor c1, RGBColor c2)
        {
            return c1.addColor(c2);
        }
        public static RGBColor operator *(double a, RGBColor c)
        {
            return c.multiplyScalar(a);
        }
        public static RGBColor operator *(RGBColor c, double a)
        {
            return c.multiplyScalar(a);
        }
        public static RGBColor operator /(RGBColor c, double a)
        {
            return c.multiplyScalar(1 / a);
        }
        public static RGBColor operator *(RGBColor c1, RGBColor c2)
        {
            return c1.multiplyColor(c2);
        }
        public static RGBColor operator ^(RGBColor c, double p)
        {
            return c.pow(p);
        }
    }
}
