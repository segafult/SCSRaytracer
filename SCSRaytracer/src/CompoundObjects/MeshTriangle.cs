//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSRaytracer
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
            normal.Normalize();
        }
        public Normal getNormal() { return normal; }
        public override bool hit(Ray r, float tmin)
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];

            float a = p0.X - p1.X;
            float b = p0.X - p2.X;
            float c = r.Direction.X;
            float d = p0.X - r.Origin.X;

            float e = p0.Y - p1.Y;
            float f = p0.Y - p2.Y;
            float g = r.Direction.Y;
            float h = p0.Y - r.Origin.Y;

            float i = p0.Z - p1.Z;
            float j = p0.Z - p2.Z;
            float k = r.Direction.Z;
            float l = p0.Z - r.Origin.Z;

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
            if (t < GlobalVars.K_EPSILON)
                return false;

            return true;
        }
        public override BoundingBox get_bounding_box()
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];
            float xmin = FastMath.min(FastMath.min(p0.X, p1.X), p2.X) - GlobalVars.K_EPSILON;
            float xmax = FastMath.max(FastMath.max(p0.X, p1.X), p2.X) + GlobalVars.K_EPSILON;
            float ymin = FastMath.min(FastMath.min(p0.Y, p1.Y), p2.Y) - GlobalVars.K_EPSILON;
            float ymax = FastMath.max(FastMath.max(p0.Y, p1.Y), p2.Y) + GlobalVars.K_EPSILON;
            float zmin = FastMath.min(FastMath.min(p0.Z, p1.Z), p2.Z) - GlobalVars.K_EPSILON;
            float zmax = FastMath.max(FastMath.max(p0.Z, p1.Z), p2.Z) + GlobalVars.K_EPSILON;
            return new BoundingBox(xmin, xmax, ymin, ymax, zmin, zmax);
        }
    }
}
