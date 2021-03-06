﻿//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;

namespace SCSRaytracer
{
    public static class FastMath
    {
        public static readonly float nearzero = 1.0e-32f;
        public static readonly float THREESIXTYINVTWOPI = (2.0f * (float)Math.PI) / 360.0f;
        public static readonly float INVTWOFITTYFI = 1.0f / 255.0f;
        public static readonly float FPI = (float)Math.PI;
        public static readonly float FINVPI = 1.0f / (float)Math.PI;
        public static readonly float FINVTWOPI = 1.0f / (float)(Math.PI * 2.0);

        public static float cbrt(float x)
        {
            if(x > 0.0)
            {
                return (float)Math.Pow(x, 1.0 / 3.0);
            }
            else if (x < 0.0)
            {
                return -(float)Math.Pow(-x, 1.0 / 3.0);
            }
            else
            {
                return 0;
            }
        }

        /********************************************************************************************************************
        *   solveQuadratic, solveCubic and solveQuartic are C# reimplementations of the code provided by Jochen             *
        *   Schwarze in Graphics Gems volume 1 (1990). See original text for a detailed explanation of their functionality. *
        *********************************************************************************************************************/
        
        /// <summary>
        /// Solves a quadratic equation of the form c[2]x^2 + c[1]x + c[0] = 0 
        /// </summary>
        /// <param name="c">Quadratic parameters</param>
        /// <param name="s">Array reference for returning roots by reference</param>
        /// <returns>Number of real roots</returns>
        static public int solveQuadratic(ref float[] c, ref float[] s)
        {
            //normal form: x^2 + px + q = 0 

            float p = c[1] / (2 * c[2]);
            float q = c[0] / c[2];
            float D = p * p - q; //Determinant

            //If determinant is 0, there's only 1 root.
            if (D > -FastMath.nearzero && D < FastMath.nearzero)
            {
                s[0] = -p;
                return 1;
            }
            //If less than 0, there are no roots
            else if( D < 0 )
            {
                return 0;
            }
            else
            {
                float sqrt_D = (float)Math.Sqrt(D);
                s[0] = sqrt_D - p;
                s[1] = -sqrt_D - p;
                return 2;
            }
        }

        /// <summary>
        /// Solves a cubic equation of the form c[3]x^3 + c[2]x^2 + c[1]x + c[0] = 0
        /// </summary>
        /// <param name="c">Cubic parameters</param>
        /// <param name="s">Array reference for returning roots by reference</param>
        /// <returns>Number of real roots</returns>
        static public int solveCubic(ref float[] c, ref float[] s)
        {
            int i, num;
            float sub;
            float A, B, C;
            float sq_A, p, q;
            float cb_p, D;
            num = 0;
            // normal form: x^3 + Ax^2 + Bx + c = 0 

            A = c[2] / c[3];
            B = c[1] / c[3];
            C = c[0] / c[3];

            //Substitute x = y - A/3 to eliminate quadric term
            // x^3 + px + q = 0

            sq_A = A * A;
            p = 1.0f / 3 * (-1.0f / 3 * sq_A + B);
            q = 1.0f / 2 * (2.0f / 27 * A * sq_A - 1.0f / 3 * A * B + C);

            // Use Cardano's formula
            cb_p = p * p * p;
            D = q * q + cb_p; //Determinant

            if(D > -FastMath.nearzero && D < FastMath.nearzero)
            {
                //One triple solution
                if(q > -FastMath.nearzero && q < FastMath.nearzero)
                {
                    s[0] = 0;
                    num = 1;
                }
                //One single and one float solution
                else
                {
                    float u = FastMath.cbrt(-q);
                    s[0] = 2 * u;
                    s[1] = -u;
                    num = 2;
                }
            }
            //Casus irreducibilis: three real solutions
            else if(D < 0.0)
            {
                float phi = 1.0f / 3 * (float)Math.Acos(-q / Math.Sqrt(-cb_p));
                float t = 2 * (float)Math.Sqrt(-p);

                s[0] = t * (float)Math.Cos(phi);
                s[1] = -t * (float)Math.Cos(phi + Math.PI / 3);
                s[2] = -t * (float)Math.Cos(phi - Math.PI / 3);
                num = 3;
            }
            //One real solution
            else
            {
                float sqrt_D = (float)Math.Sqrt(D);
                float u = FastMath.cbrt(sqrt_D - q);
                float v = -FastMath.cbrt(sqrt_D+ q);

                s[0] = u + v;
                num = 1;
            }

            //Resubstitute
            sub = 1.0f / 3 * A;
            for(i = 0;i < num;++i)
            {
                s[i] -= sub;
            }
            return num;
        }

        /// <summary>
        /// Solves a quartic equation of the form c[4]x^4 + c[3]x^3 + c[2]x^2 + c[1]x + c[0] = 0
        /// </summary>
        /// <param name="c">Quartic parameters</param>
        /// <param name="s">Array reference for returning roots by reference</param>
        /// <returns></returns>
        static public int SolveQuartic(float[] c, float[] s)
        {
            float[] coeffs = new float[4];
            float z, u, v, sub;
            float A, B, C, D;
            float sq_A, p, q, r;
            int i, num;
            //num = 0;
            // Normal form: x^4 + Ax^3 + Bx^2 + Cx + D = 0

            A = c[3] / c[4];
            B = c[2] / c[4];
            C = c[1] / c[4];
            D = c[0] / c[4];

            //Substitute x = y - A/4 to eliminate cubic term
            //x^4 + px^2 + qx + r = 0

            sq_A = A * A;
            p = -3.0f / 8 * sq_A + B;
            q = 1.0f / 8 * sq_A * A - 1.0f / 2 * A * B + C;
            r = -3.0f / 256 * sq_A * sq_A + 1.0f / 16 * sq_A * B - 1.0f / 4 * A * C + D;

            if(r > -FastMath.nearzero && r < FastMath.nearzero)
            {
                //No absolute term: y(y^3 + py + q) = 0
                coeffs[0] = q;
                coeffs[1] = p;
                coeffs[2] = 0;
                coeffs[3] = 1;

                num = solveCubic(ref coeffs, ref s);
                s[num++] = 0;
            }
            else
            {
                //Solve the resultant cubic

                coeffs[0] = 1.0f / 2 * r * p - 1.0f / 8 * q * q;
                coeffs[1] = -r;
                coeffs[2] = -1.0f / 2 * p;
                coeffs[3] = 1;

                solveCubic(ref coeffs, ref s);

                //Take the one real solution
                z = s[0];

                //To build two quadratic equations
                u = z * z - r;
                v = 2 * z - p;
                if(u > -FastMath.nearzero && u < FastMath.nearzero)
                {
                    u = 0;
                }
                else if(u > 0)
                {
                    u = (float)Math.Sqrt(u);
                }
                else
                {
                    return 0;
                }

                if(v > -FastMath.nearzero && v < FastMath.nearzero)
                {
                    v = 0;
                }
                else if(v > 0) 
                {
                    v = (float)Math.Sqrt(v);
                }
                else
                {
                    return 0;
                }

                coeffs[0] = z - u;
                coeffs[1] = q < 0 ? -v : v;
                coeffs[2] = 1;
                    
                num = solveQuadratic(ref coeffs, ref s);

                coeffs[0] = z + u;
                coeffs[1] = q < 0 ? v : -v;
                coeffs[2] = 1;

                float[] temps = new float[3];
                int looplength = solveQuadratic(ref coeffs, ref temps);
                for(i = 0; i < looplength; i++)
                {
                    s[i + num] = temps[i];
                }
                num += looplength;
                
            }

            //Resubstitute
            sub = 1.0f / 4 * A;
            for(i = 0; i < num; ++i)
            {
                s[i] -= sub;
            }

            return num;
        }

        static public float min(float a, float b)
        {
            return (a < b) ? a : b;
        }
        static public float max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public float clamp(float x, float xmin, float xmax)
        {
            if (x < xmin)
            {
                return xmin;
            }
            else if (x > xmax)
            {
                return xmax;
            }
            else { return x; }
        }
    }
}
