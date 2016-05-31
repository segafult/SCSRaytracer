//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace SCSRaytracer
{
    public struct RGBColor
    {
        public static Vector3 BLACK = new Vector3(0.0f, 0.0f, 0.0f);
        public static Vector3 WHITE = new Vector3(255.0f, 255.0f, 255.0f);
        private Vector3 _vals;
        public Vector3 Values
        {
            get
            {
                return _vals;
            }
            set
            {
                _vals = value;
            }
        }
        public float R
        {
            get
            {
                return _vals.X;
            }
            set
            {
                _vals.X = value;
            }
        }
        public float G
        {
            get
            {
                return _vals.Y;
            }
            set
            {
                _vals.Y = value;
            }
        }
        public float B
        {
            get
            {
                return _vals.Z;
            }
            set
            {
                _vals.Z = value;
            }
        }

        public override string ToString()
        {
            return "#" + Convert.ToByte(_vals.X * 255.0).ToString("x") +
                Convert.ToByte(_vals.Y * 255.0).ToString("x") + 
                Convert.ToByte(_vals.Z * 255.0).ToString("x");
        }

        public RGBColor (float red, float green, float blue)
        {
            _vals = new Vector3(red, green, blue);
        }
        public RGBColor (Vector3 v)
        {
            _vals = v;
        }
        public RGBColor (System.Drawing.Color col)
        {
            _vals = new Vector3(col.R * FastMath.INVTWOFITTYFI, col.G * FastMath.INVTWOFITTYFI, col.B * FastMath.INVTWOFITTYFI);
        }
        public RGBColor (SFML.Graphics.Color col)
        {
            _vals = new Vector3(col.R * FastMath.INVTWOFITTYFI, col.G * FastMath.INVTWOFITTYFI, col.B * FastMath.INVTWOFITTYFI);
        }
        //Copy constructor
        public RGBColor (RGBColor color)
        {
            _vals = color._vals;
        }  
        
        public RGBColor clamp()
        {
            return new RGBColor(Vector3.Clamp(_vals, BLACK, WHITE));
        }

        ///
        ///Operator overloads
        ///
        //Color addition
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator +(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1._vals + c2._vals);
        }

        //Scalar multiplication of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(float a, RGBColor c)
        {
            return new RGBColor(c._vals * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c, float a)
        {
            return new RGBColor(c._vals * a);
        }

        //Scalar division of a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator /(RGBColor c, float a)
        {
            return new RGBColor(c._vals / a);
        }

        //Multiplication of color with a color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator *(RGBColor c1, RGBColor c2)
        {
            return new RGBColor(c1._vals * c2._vals);
        }

        //Raising color to a power
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RGBColor operator ^(RGBColor c, float p)
        {
            return new RGBColor((float)Math.Pow(c._vals.X, p), (float)Math.Pow(c._vals.Y, p), (float)Math.Pow(c._vals.Z, p));
        }
    }
}
