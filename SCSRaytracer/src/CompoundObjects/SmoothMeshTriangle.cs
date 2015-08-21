//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class SmoothMeshTriangle : MeshTriangle
    {
        public SmoothMeshTriangle(Mesh parent) : base(parent)
        {

        }

        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
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

            tmin = t;
            sr.normal = interpolate_normal(beta, gamma);
            sr.hit_point_local = r.origin + t * r.direction;
            return true;
        }

        private Normal interpolate_normal(float beta, float gamma)
        {
            Normal interpolated = (1 - beta - gamma) * parent.normals[index0] +
                beta * parent.normals[index1] +
                gamma * parent.normals[index2];
            interpolated.normalize();

            return interpolated;
        }
    }
}
