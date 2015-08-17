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
    abstract class MeshTriangle : RenderableObject
    {
        protected int index0, index1, index2;
        protected Mesh parent;
        protected Normal normal;

        public MeshTriangle(Mesh p_arg)
        {
            parent = p_arg;
        }

        public void setVertexIndices(int ind0, int ind1, int ind2)
        {
            index0 = ind0;
            index1 = ind1;
            index2 = ind2;
            compute_normal();
        }
        private void compute_normal()
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];

            //Normal is the cross product of two sides of a triangle
            Vect3D v0 = p1 - p0;
            Vect3D v1 = p2 - p0;
            Vect3D rawNormal = v0 ^ v1;

            normal = new Normal(rawNormal);
            normal.normalize();
        }
        public Normal getNormal() { return normal; }
        public override bool hit(Ray r, float tmin)
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];

            float a = p0.coords.X - p1.coords.X;
            float b = p0.coords.X - p2.coords.X;
            float c = r.direction.coords.X;
            float d = p0.coords.X - r.origin.coords.X;

            float e = p0.coords.Y - p1.coords.Y;
            float f = p0.coords.Y - p2.coords.Y;
            float g = r.direction.coords.Y;
            float h = p0.coords.Y - r.origin.coords.Y;

            float i = p0.coords.Z - p1.coords.Z;
            float j = p0.coords.Z - p2.coords.Z;
            float k = r.direction.coords.Z;
            float l = p0.coords.Z - r.origin.coords.Z;

            float m = f * k - g * j;
            float n = h * k - g * l;
            float p = f * l - h * j;
            float q = g * i - e * k;
            float s = e * j - f * i;

            float invDenom = 1.0f / (a * m + b * q + c * s);

            float beta = invDenom * (d * m - b * n - c * p);
            if (beta < 0.0)
                return false;

            float rd = e * l - h * i;
            float gamma = invDenom * (a * n + d * q + c * rd);
            if (gamma < 0.0)
                return false;
            if (beta + gamma > 1.0)
                return false;

            //Hit!
            float t = invDenom * (a * p - b * rd + d * s);
            if (t < GlobalVars.kEpsilon)
                return false;

            return true;
        }
        public override BoundingBox get_bounding_box()
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];
            float xmin = FastMath.min(FastMath.min(p0.coords.X, p1.coords.X), p2.coords.X) - GlobalVars.kEpsilon;
            float xmax = FastMath.max(FastMath.max(p0.coords.X, p1.coords.X), p2.coords.X) + GlobalVars.kEpsilon;
            float ymin = FastMath.min(FastMath.min(p0.coords.Y, p1.coords.Y), p2.coords.Y) - GlobalVars.kEpsilon;
            float ymax = FastMath.max(FastMath.max(p0.coords.Y, p1.coords.Y), p2.coords.Y) + GlobalVars.kEpsilon;
            float zmin = FastMath.min(FastMath.min(p0.coords.Z, p1.coords.Z), p2.coords.Z) - GlobalVars.kEpsilon;
            float zmax = FastMath.max(FastMath.max(p0.coords.Z, p1.coords.Z), p2.coords.Z) + GlobalVars.kEpsilon;
            return new BoundingBox(xmin, xmax, ymin, ymax, zmin, zmax);
        }
    }
}
