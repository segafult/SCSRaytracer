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

namespace RayTracer
{
    /// <summary>
    /// Triangle! ^______^
    /// </summary>
    public class Triangle : RenderableObject
    {
        private Point3D v1, v2, v3; //Vertexes

        public Triangle()
        {
            v1 = new Point3D(100, 0, 0);
            v2 = new Point3D(-100, 0, 0);
            v3 = new Point3D(0, 100, 0);
        }
        public Triangle(Point3D v1_arg, Point3D v2_arg, Point3D v3_arg)
        {
            //Shallow copy to save memory. Mesh container class handles duplicate vertices for memory savings.
            v1 = v1_arg;
            v2 = v2_arg;
            v3 = v3_arg;
        }

        public override string ToString()
        {
            return "Triangle primitive:\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.getMaterial().id + "\n" +
                "  Vertices: " + v1.ToString() + v2.ToString() + v3.ToString();
        }
        //Gets and sets
        public void setVertices(Point3D v1_arg, Point3D v2_arg, Point3D v3_arg)
        {
            v1 = v1_arg;
            v2 = v2_arg;
            v3 = v3_arg;
        }
        public void setSingleVertices(int vert, Point3D v)
        {
            switch (vert)
            {
                case 1: v1 = v;
                    break;
                case 2: v2 = v;
                    break;
                case 3: v3 = v;
                    break;
            }
        }
        public Point3D getSingleVertices(int vert)
        {
            switch(vert)
            {
                case 1: return v1;
                case 2: return v2;
                case 3: return v3;
            }
            return null;
        }
        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            //Calculate ray intersection with the Moller-Trumbore algorithm
            Vect3D e1, e2; //Edges
            Vect3D P, Q, T;
            double det, inv_det, u, v, t;

            e1 = v2 - v1;
            e2 = v3 - v1; //Vectors for the edges shared by vertex 1

            P = r.direction ^ e2;
            det = e1 * P; //Determinant calculation

            //If determinant is almost zero, then ray is parallel
            if(det > -GlobalVars.kEpsilon && det < GlobalVars.kEpsilon)
            {
                return false;
            }

            inv_det = 1.0 / det;

            //Distance from v1 to ray origin
            T = r.origin - v1;

            //U parameter calculation and test
            u = (T * P) * inv_det;
            //If u is negative or greater than 1, then intersection is outside of triangle
            if(u < 0.0 || u > 1.0)
            {
                return false;
            }

            Q = T ^ e1;
            //V parameter calculation and test
            v = (r.direction * Q) * inv_det;
            //if v is negative or v+u is greater than 1, then intersection is outside of triangle
            if(v < 0.0 || u+v > 1.0 )
            {
                return false;
            }

            t = e2 * Q * inv_det;

            if(t > GlobalVars.kEpsilon && t < tmin)
            {
                //Ray hit something and intersection is in front of camera
                tmin = t;
                //Calculate normal, and ensure it's facing the direction that the ray came from
                Vect3D triNorm = (v2 - v1) ^ (v3 - v1);
                double triDotProd = triNorm * r.direction;
                if(triDotProd < 0.0)
                {
                    sr.normal = new Normal(triNorm);
                }
                else
                {
                    sr.normal = new Normal(-triNorm);
                }
                sr.normal.normalize();
                sr.hit_point_local = r.origin + t * r.direction;
                sr.hit_an_object = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool hit(Ray r, double tmin)
        {
            //Calculate ray intersection with the Moller-Trumbore algorithm

            Vect3D e1, e2; //Edges
            Vect3D P, Q, T;
            double det, inv_det, u, v, t;

            e1 = v2 - v1;
            e2 = v3 - v1; //Vectors for the edges shared by vertex 1

            P = r.direction ^ e2;
            det = e1 * P; //Determinant calculation

            //If determinant is almost zero, then ray is parallel
            if (det > -GlobalVars.kEpsilon && det < GlobalVars.kEpsilon)
            {
                return false;
            }

            inv_det = 1.0 / det;

            //Distance from v1 to ray origin
            T = r.origin - v1;

            //U parameter calculation and test
            u = (T * P) * inv_det;
            //If u is negative or greater than 1, then intersection is outside of triangle
            if (u < 0.0 || u > 1.0)
            {
                return false;
            }

            Q = T ^ e1;
            //V parameter calculation and test
            v = (r.direction * Q) * inv_det;
            //if v is negative or v+u is greater than 1, then intersection is outside of triangle
            if (v < 0.0 || u + v > 1.0)
            {
                return false;
            }

            t = e2 * Q * inv_det;

            if (t > GlobalVars.kEpsilon && t < tmin)
            {
                return true;
            }
            else return false;
        }
    }
}
