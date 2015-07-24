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

namespace RayTracer
{
    public class Torus : RenderableObject
    {
        private double a, b;
       
        private BoundingBox bb;

        public Torus()
        {
            a = 2.0;
            b = 1.0;

            bb = new BoundingBox(-a + b, a + b, -b, b, -a + b, a + b);
        }

        public Torus(double a_arg, double b_arg)
        {
            a = a_arg;
            b = b_arg;

            bb = new BoundingBox(-a - b, a + b, -b, b, -a - b, a + b);
        }

        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            if(!bb.hit(r, tmin))
            {
                return false;
            }
            /// Hit point on a torus as distance from point r.origin along vector r.direction
            /// can be solved for as a quartic equation of the form c4t^4 + c3t^3 + c2t^2 + c1t + c0 = 0
            /// where:
            /// c4 = (xd^2 + yd^2 + zd^2)^2
            /// c3 = 4(xd^2 + yd^2 + zd^2)(xoxd+yoyd+zozd)
            /// c2 = 2(xd^2 + yd^2 + zd^2)(xo^2+yo^2+zo^2-(a^2 + b^2))+4(xoxd + yoyd + zozd)^2 + 4a^2yd^2
            /// c1 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))(xoxd + yoyd + zozd) + 8a^2yoyd
            /// c0 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))^2 - 4a^2(b^2 - yo^2)
            /// 
            ///Using quartic solving algorithm
            /// 
            double x1 = r.origin.xcoord; double y1 = r.origin.ycoord; double z1 = r.origin.zcoord;
            double d1 = r.direction.xcoord; double d2 = r.direction.ycoord; double d3 = r.direction.zcoord;

            double[] coefficients = new double[5];
            double[] roots = new double[4];

            double sum_d_sqrd = d1 * d1 + d2 * d2 + d3 * d3;
            double e = x1 * x1 + y1 * y1 + z1 * z1 - a * a - b * b;
            double f = x1 * d1 + y1 * d2 + z1 * d3;
            double four_a_sqrd = 4.0 * a * a;


            coefficients[0] = e * e - four_a_sqrd * (b * b - y1 * y1);
            coefficients[1] = 4.0 * f * e + 2.0 * four_a_sqrd * y1 * d2;
            coefficients[2] = 2.0 * sum_d_sqrd * e + 4.0 * f * f + four_a_sqrd * d2 * d2;
            coefficients[3] = 4.0 * sum_d_sqrd * f;
            coefficients[4] = sum_d_sqrd * sum_d_sqrd;

            //Find roots
            int numroots = FastMath.solveQuartic(coefficients, roots);
           
            bool hit = false;
            double t = tmin;
            
            if (numroots == 0)
            {
                
                return false;
            }
            
            //Find the smallest root
            for(int i = 0;i< numroots;i++)
            {
                if(roots[i] > GlobalVars.kEpsilon && roots[i] < t)
                {
                    hit = true;
                    t = roots[i];
                }
            }

            if(!hit)
            {
                
                return false;
            }

            tmin = t;
            sr.hit_point_local = r.origin + (t * r.direction);
            //Compute the normal at the hit point (see Thomas and Finney, 1996)
            
            sr.normal = calcNormal(sr.hit_point_local);

            return hit;
        }

        public override bool hit(Ray r, double tmin)
        {
            /// Hit point on a torus as distance from point r.origin along vector r.direction
            /// can be solved for as a quartic equation of the form c4t^4 + c3t^3 + c2t^2 + c1t + c0 = 0
            /// where:
            /// c4 = (xd^2 + yd^2 + zd^2)^2
            /// c3 = 4(xd^2 + yd^2 + zd^2)(xoxd+yoyd+zozd)
            /// c2 = 2(xd^2 + yd^2 + zd^2)(xo^2+yo^2+zo^2-(a^2 + b^2))+4(xoxd + yoyd + zozd)^2 + 4a^2yd^2
            /// c1 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))(xoxd + yoyd + zozd) + 8a^2yoyd
            /// c0 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))^2 - 4a^2(b^2 - yo^2)
            /// 
            ///Using quartic solving algorithm
            /// 
            double x1 = r.origin.xcoord; double y1 = r.origin.ycoord; double z1 = r.origin.zcoord;
            double d1 = r.direction.xcoord; double d2 = r.direction.ycoord; double d3 = r.direction.zcoord;

            double[] coefficients = new double[5];
            double[] roots = new double[4];

            double sum_d_sqrd = d1 * d1 + d2 * d2 + d3 * d3;
            double e = x1 * x1 + y1 * y1 + z1 * z1 - a * a - b * b;
            double f = x1 * d1 + y1 * d2 + z1 * d3;
            double four_a_sqrd = 4.0 * a * a;


            coefficients[0] = e * e - four_a_sqrd * (b * b - y1 * y1);
            coefficients[1] = 4.0 * f * e + 2.0 * four_a_sqrd * y1 * d2;
            coefficients[2] = 2.0 * sum_d_sqrd * e + 4.0 * f * f + four_a_sqrd * d2 * d2;
            coefficients[3] = 4.0 * sum_d_sqrd * f;
            coefficients[4] = sum_d_sqrd * sum_d_sqrd;

            //Find roots
            int numroots = FastMath.solveQuartic(coefficients, roots);

            bool hit = false;
            double t = GlobalVars.kHugeValue;

            if (numroots == 0)
            {

                return false;
            }

            //Find the smallest root
            for (int i = 0; i < numroots; i++)
            {
                if (roots[i] > GlobalVars.kEpsilon && roots[i] < tmin)
                {
                    return true;
                }
            }

            return false;
        }

        private Normal calcNormal(Point3D point)
        {
            Normal result = new Normal();

            double param_squared = a * a + b * b;

            double x = point.xcoord;
            double y = point.ycoord;
            double z = point.zcoord;
            double sum_squared = x * x + y * y + z * z;

            result.xcoord = 4.0 * x * (sum_squared - param_squared);
            result.ycoord = 4.0 * y * (sum_squared - param_squared + 2.0 * a * a);
            result.zcoord = 4.0 * z * (sum_squared - param_squared);
            result.normalize();

            return result;
        }
    }
}
