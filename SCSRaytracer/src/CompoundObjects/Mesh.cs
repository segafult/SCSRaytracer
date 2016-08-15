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
using System.Xml;

namespace SCSRaytracer
{
    class Mesh : UniformGrid
    {
        public List<Point3D> vertices;
        public List<Normal> normals;
        public List<List<int>> vertexFaces; //List of faces shared by each vertex. Used for calculating normals.
        public List<float> u;
        public List<float> v;

        public int countVertices;
        public int countTriangles;

        MeshLoader loader;

        public Mesh() : base()
        {
            vertices = new List<Point3D>();
            normals = new List<Normal>();
            vertexFaces = new List<List<int>>();
            u = new List<float>();
            v = new List<float>();

            countVertices = 0;
            countTriangles = 0;
        }

        public void loadFromFile(string filename, bool smooth)
        {
            string extension = filename.Substring(filename.LastIndexOf('.') + 1);
            if (extension.ToLower().Equals("off"))
                loader = new OFFLoader();
            else
            {
                Console.WriteLine("Unknown mesh filename extension: " + extension);
                return;
            }

            if(!loader.OpenFile(filename))
            {
                Console.WriteLine("Failed to open file: " + filename);
                return;
            }

            loader.ParseVertices(this);
            loader.ParseFaces(this, smooth);
        }

        public Normal NormalForFace(int index)
        {
            return ((MeshTriangle)containedObjects[index]).Normal;
        }

        public static Mesh LoadMesh(XmlElement def)
        {
            Mesh toReturn = new Mesh();

            if(def.HasAttribute("filename"))
            {
                string str_file = def.GetAttribute("filename");
                bool smooth = false;
                if (def.HasAttribute("smooth"))
                    smooth = Convert.ToBoolean(def.GetAttribute("smooth"));

                toReturn.loadFromFile(str_file, smooth);
            }

            toReturn.SetupCells();

            return toReturn;
        }
    }
}
