//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Collections.Generic;

namespace SCSRaytracer
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
