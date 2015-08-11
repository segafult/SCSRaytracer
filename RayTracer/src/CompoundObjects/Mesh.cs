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
    class Mesh : UniformGrid
    {
        public List<Point3D> vertices;
        public List<Normal> normals;
        public List<List<int>> vertex_faces; //List of faces shared by each vertex. Used for calculating normals.
        public List<double> u;
        public List<double> v;

        public int num_verts;
        public int num_triangles;

        MeshLoader loader;

        public Mesh() : base()
        {
            vertices = new List<Point3D>();
            normals = new List<Normal>();
            vertex_faces = new List<List<int>>();
            u = new List<double>();
            v = new List<double>();

            num_verts = 0;
            num_triangles = 0;
        }

        public void loadFromFile(string filename)
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
            loader.parseFaces(this, true);
        }

        public Normal normalForFace(int index)
        {
            return ((MeshTriangle)objs[index]).getNormal();
        }
    }
}
