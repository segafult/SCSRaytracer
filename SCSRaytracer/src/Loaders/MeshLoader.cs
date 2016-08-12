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
        protected BoundingBox _boundingBox;
        virtual public BoundingBox BoundingBox
        {
            get
            {
                return _boundingBox;
            }
        }

        abstract public bool OpenFile(string filename);
        abstract public void ParseVertices(Mesh parent);
        abstract public void ParseFaces(Mesh parent, bool smooth);
        virtual public void ParseUV(Mesh parent) {  }
        //virtual public BoundingBox getBoundingBox() { return _boundingBox; }

        protected void CalculateNormals(Mesh parent)
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
                Normal normalSum = new Normal(0,0,0);
                for(int j = 0; j < faceList.Count; j++)
                {
                    normalSum += parent.normalForFace(faceList[j]);
                }
                normalSum.Normalize();
                parent.normals[i] = normalSum;
            }
        }
    }
}
