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
    class FlatMeshTriangle : MeshTriangle
    {
        public FlatMeshTriangle(Mesh parent) : base(parent)
        {

        }

        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];

            double a = p0.xcoord - p1.xcoord;
            double b = p0.xcoord - p2.xcoord;
            double c = r.direction.xcoord;
            double d = p0.xcoord - r.origin.xcoord;

            double e = p0.ycoord - p1.ycoord;
            double f = p0.ycoord - p2.ycoord;
            double g = r.direction.ycoord;
            double h = p0.ycoord - r.origin.ycoord;

            double i = p0.zcoord - p1.zcoord;
            double j = p0.zcoord - p2.zcoord;
            double k = r.direction.zcoord;
            double l = p0.zcoord - r.origin.zcoord;

            double m = f * k - g * j;
            double n = h * k - g * l;
            double p = f * l - h * j;
            double q = g * i - e * k;
            double s = e * j - f * i;

            double invDenom = 1.0 / (a * m + b * q + c * s);

            double beta = invDenom * (d * m - b * n - c * p);
            if (beta < 0.0)
                return false;

            double rd = e * l - h * i;
            double gamma = invDenom * (a * n + d * q + c * rd);
            if (gamma < 0.0)
                return false;
            if (beta + gamma > 1.0)
                return false;

            //Hit!
            double t = invDenom * (a * p - b * rd + d * s);
            if (t < GlobalVars.kEpsilon)
                return false;

            tmin = t;
            sr.normal = normal;
            sr.hit_point_local = r.origin + t * r.direction;
            return true;
        }
    }
}
