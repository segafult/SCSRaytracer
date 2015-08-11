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

using System.Collections.Generic;

namespace RayTracer
{
    abstract class MeshLoader
    {
        protected BoundingBox bb;

        abstract public bool openFile(string filename);
        abstract public void parseVertices(Mesh parent);
        abstract public void parseFaces(Mesh parent, bool smooth);
        virtual public void parseUV(Mesh parent) {  }
        virtual public BoundingBox getBoundingBox() { return bb; }
        protected void calculateNormals(Mesh parent)
        {
            if(GlobalVars.verbose)
            {
                System.Console.WriteLine("Calculating vertex normals for mesh " + parent.id);
            }
            //Get the list of faces attached to a given vertex
            for (int i = 0; i < parent.num_verts; i++) 
            {
                //Sum together all the normals of the faces attached to this vertex
                List<int> faceList = parent.vertex_faces[i];
                Normal sum_norm = new Normal(0,0,0);
                for(int j = 0; j < faceList.Count; j++)
                {
                    sum_norm += parent.normalForFace(faceList[j]);
                }
                sum_norm.normalize();
                parent.normals[i] = sum_norm;
            }
        }
    }
}
