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
    class TriangleMesh : RenderableObject
    {
        List<Triangle> mesh_faces;
        List<Point3D> mesh_vertices;
        BoundingBox bb;

        public TriangleMesh()
        {
            bb = new BoundingBox(0, 0, 0, 0, 0, 0);
            mesh_faces = new List<Triangle>();
            mesh_vertices = new List<Point3D>();
        }

        public void loadFromFile(string filename)
        {
            //Set up mesh loader

            MeshLoader loader;
            //Determine the filetype for loading
            string filetype = filename.Substring(filename.LastIndexOf('.') + 1);


            if(filetype.ToLower().Equals("off"))
            {
                loader = new OFFLoader();
            }
            else
            {
                Console.WriteLine("Invalid filetype: " + filetype);
                return;
            }

            if(loader.openFile(filename))
            {
                //Extract all the necessary data from the files
                mesh_vertices = loader.parseVertices();
                mesh_faces = loader.parseFaces(mesh_vertices);
                bb = loader.getBoundingBox();
            }
            else
            {
                Console.WriteLine("Loader failed to open file " + filename);
            }
        }

        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            //Check for intersection with bounding box
            if(!bb.hit(r,tmin))
            {
                return false;
            }

            ShadeRec dummySr = new ShadeRec(sr.w);
            int numFaces = mesh_faces.Count;
            double t = GlobalVars.kHugeValue;
            bool hit_a_face = false;
            
            //Traverse list of triangles and intersect to find tmin
            for(int i = 0; i < numFaces; i++)
            {
                if(mesh_faces[i].hit(r, ref t, ref sr) && (t < tmin))
                {
                    hit_a_face = true;
                    tmin = t;
                }
            }

            return hit_a_face;
        }

        public override bool hit(Ray r, double tmin)
        {
            return base.hit(r, tmin);
        }
    }
}
