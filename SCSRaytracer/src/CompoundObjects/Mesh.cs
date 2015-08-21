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
        public List<List<int>> vertex_faces; //List of faces shared by each vertex. Used for calculating normals.
        public List<float> u;
        public List<float> v;

        public int num_verts;
        public int num_triangles;

        MeshLoader loader;

        public Mesh() : base()
        {
            vertices = new List<Point3D>();
            normals = new List<Normal>();
            vertex_faces = new List<List<int>>();
            u = new List<float>();
            v = new List<float>();

            num_verts = 0;
            num_triangles = 0;
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

            if(!loader.openFile(filename))
            {
                Console.WriteLine("Failed to open file: " + filename);
                return;
            }

            loader.parseVertices(this);
            loader.parseFaces(this, smooth);
        }

        public Normal normalForFace(int index)
        {
            return ((MeshTriangle)objs[index]).getNormal();
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

            toReturn.setup_cells();

            return toReturn;
        }
    }
}
