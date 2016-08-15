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
    class FlatMeshTriangle : MeshTriangle
    {
        public FlatMeshTriangle(Mesh parent) : base(parent)
        {

        }

        public override bool Hit(Ray ray, ref float tMin, ref ShadeRec sr)
        {
            Point3D p0 = parent.vertices[index0];
            Point3D p1 = parent.vertices[index1];
            Point3D p2 = parent.vertices[index2];

            float a = p0.X - p1.X;
            float b = p0.X - p2.X;
            float c = ray.Direction.X;
            float d = p0.X - ray.Origin.X;

            float e = p0.Y - p1.Y;
            float f = p0.Y - p2.Y;
            float g = ray.Direction.Y;
            float h = p0.Y - ray.Origin.Y;

            float i = p0.Z - p1.Z;
            float j = p0.Z - p2.Z;
            float k = ray.Direction.Z;
            float l = p0.Z - ray.Origin.Z;

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

            tMin = t;
            sr.Normal = _normal;
            sr.HitPointLocal = ray.Origin + t * ray.Direction;
            return true;
        }
    }
}
