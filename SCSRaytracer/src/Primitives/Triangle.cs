//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Xml;

namespace SCSRaytracer
{
    /// <summary>
    /// Simple representation of a triangle which stores its own vertices as points.
    /// </summary>
    sealed class Triangle : RenderableObject
    {
        private Point3D Vertex1, Vertex2, Vertex3; //Vertexes

        public Triangle()
        {
            Vertex1 = new Point3D(100, 0, 0);
            Vertex2 = new Point3D(-100, 0, 0);
            Vertex3 = new Point3D(0, 100, 0);
        }
        public Triangle(Point3D vertex1Arg, Point3D vertex2Arg, Point3D vertex3Arg)
        {
            //Shallow copy to save memory. Mesh container class handles duplicate vertices for memory savings.
            Vertex1 = vertex1Arg;
            Vertex2 = vertex2Arg;
            Vertex3 = vertex3Arg;
        }

        public override string ToString()
        {
            return "Triangle primitive:\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.Material.id + "\n" +
                "  Vertices: " + Vertex1.ToString() + Vertex2.ToString() + Vertex3.ToString();
        }
        //Gets and sets
        public void SetVertices(Point3D v1_arg, Point3D v2_arg, Point3D v3_arg)
        {
            Vertex1 = v1_arg;
            Vertex2 = v2_arg;
            Vertex3 = v3_arg;
        }
        public void SetSingleVertices(int vert, Point3D v)
        {
            switch (vert)
            {
                case 1: Vertex1 = v;
                    break;
                case 2: Vertex2 = v;
                    break;
                case 3: Vertex3 = v;
                    break;
            }
        }
        public Point3D GetSingleVertices(int vert)
        {
            switch(vert)
            {
                case 1: return Vertex1;
                case 2: return Vertex2;
                case 3: return Vertex3;
            }
            return new Point3D(0,0,0);
        }
        public override bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            //Calculate ray intersection with the Moller-Trumbore algorithm
            Vect3D edge1, edge2; //Edges
            Vect3D P, Q, T;
            float determinant, invDeterminant, u, v, t;

            edge1 = Vertex2 - Vertex1;
            edge2 = Vertex3 - Vertex1; //Vectors for the edges shared by vertex 1

            P = r.Direction ^ edge2;
            determinant = edge1 * P; //Determinant calculation

            //If determinant is almost zero, then ray is parallel
            if(determinant > -GlobalVars.K_EPSILON && determinant < GlobalVars.K_EPSILON)
            {
                return false;
            }

            invDeterminant = 1.0f / determinant;

            //Distance from v1 to ray origin
            T = r.Origin - Vertex1;

            //U parameter calculation and test
            u = (T * P) * invDeterminant;
            //If u is negative or greater than 1, then intersection is outside of triangle
            if(u < 0.0 || u > 1.0)
            {
                return false;
            }

            Q = T ^ edge1;
            //V parameter calculation and test
            v = (r.Direction * Q) * invDeterminant;
            //if v is negative or v+u is greater than 1, then intersection is outside of triangle
            if(v < 0.0 || u+v > 1.0 )
            {
                return false;
            }

            t = edge2 * Q * invDeterminant;

            if(t > GlobalVars.K_EPSILON && t < tmin)
            {
                //Ray hit something and intersection is in front of camera
                tmin = t;
                //Calculate normal, and ensure it's facing the direction that the ray came from
                Vect3D triNorm = (Vertex2 - Vertex1) ^ (Vertex3 - Vertex1);
                float triDotProd = triNorm * r.Direction;
                if(triDotProd < 0.0)
                {
                    sr.Normal = new Normal(triNorm);
                }
                else
                {
                    sr.Normal = new Normal(-triNorm);
                }
                sr.Normal.Normalize();
                sr.HitPointLocal = r.Origin + t * r.Direction;
                sr.ObjectMaterial = _material;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Hit(Ray r, float tmin)
        {
            //Calculate ray intersection with the Moller-Trumbore algorithm

            Vect3D e1, e2; //Edges
            Vect3D P, Q, T;
            float determinant, invDeterminant, u, v, t;

            e1 = Vertex2 - Vertex1;
            e2 = Vertex3 - Vertex1; //Vectors for the edges shared by vertex 1

            P = r.Direction ^ e2;
            determinant = e1 * P; //Determinant calculation

            //If determinant is almost zero, then ray is parallel
            if (determinant > -GlobalVars.K_EPSILON && determinant < GlobalVars.K_EPSILON)
            {
                return false;
            }

            invDeterminant = 1.0f / determinant;

            //Distance from v1 to ray origin
            T = r.Origin - Vertex1;

            //U parameter calculation and test
            u = (T * P) * invDeterminant;
            //If u is negative or greater than 1, then intersection is outside of triangle
            if (u < 0.0 || u > 1.0)
            {
                return false;
            }

            Q = T ^ e1;
            //V parameter calculation and test
            v = (r.Direction * Q) * invDeterminant;
            //if v is negative or v+u is greater than 1, then intersection is outside of triangle
            if (v < 0.0 || u + v > 1.0)
            {
                return false;
            }

            t = e2 * Q * invDeterminant;

            if (t > GlobalVars.K_EPSILON && t < tmin)
            {
                return true;
            }
            else return false;
        }

        public static Triangle LoadTrianglePrimitive(XmlElement def)
        {
            Triangle toReturn = new Triangle();

            //Check if a list of vertices have been defined
            XmlNode vertRoot = def.SelectSingleNode("vertices");
            if (vertRoot != null)
            {
                //Get a list of all defined points
                XmlNodeList vertList = vertRoot.SelectNodes("point");
                //Need to be 3 vertices defined
                if (vertList.Count == 3)
                {
                    List<Point3D> verts_objs = new List<Point3D>(3);
                    foreach (XmlNode element in vertList)
                    {
                        verts_objs.Add(Point3D.FromCsv(((XmlText)element.FirstChild).Data));
                    }
                    toReturn.SetVertices(verts_objs[0], verts_objs[1], verts_objs[2]);
                }
                else
                {
                    Console.WriteLine("Error: Exactly 3 vertices must be defined for a triangle primitive if <vertices> tags present.");
                }
            }

            return toReturn;
        }
        /*
        public override BoundingBox get_bounding_box()
        {
            //Find the smallest x coordinate
            float xmin = GlobalVars.kHugeValue;
            //xmin = (v1.coords.X < )
        }
        */
    }
}
