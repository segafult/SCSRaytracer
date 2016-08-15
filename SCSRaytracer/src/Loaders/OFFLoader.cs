//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;

namespace SCSRaytracer
{
    class OFFLoader : MeshLoader
    {
        string[] fileLines;

        int indexOfHeader;
        int indexOfVertices;
        int indexOfFaces;
        
        int countVertices;
        int countFaces;
        

        public override bool OpenFile(string filename)
        {
            //if(System.IO.File.Exists(filename))
            //{
                fileLines = System.IO.File.ReadAllLines(filename);

                //To be a valid .off file, must start with 'OFF'
                if (fileLines[0].Equals("OFF"))
                {
                    string header = this.GetOFFHeader();

                    //Tokenize header and extract file info
                    string[] headerData = header.Split(' ');
                    countVertices = Convert.ToInt32(headerData[0]);
                    countFaces = Convert.ToInt32(headerData[1]);

                    //Set the indices of data based on extracted header info
                    indexOfVertices = indexOfHeader + 1;
                    indexOfFaces = indexOfVertices + countVertices;

                    //File is open and all relevant info extracted, return true
                    return true;
                }
                else
                {
                    return false;
                }
            //}
            //else
            //{
            //    return false;
            //}
        }

        public override void ParseVertices(Mesh parent)
        {
            int pastLastIndex = indexOfVertices + countVertices;

            for (int i = indexOfVertices; i < pastLastIndex; i++)
            {
                //Tokenize each line...
                string[] vertices = fileLines[i].Split(' ');

                float x = Convert.ToSingle(vertices[0]);
                float y = Convert.ToSingle(vertices[1]);
                float z = Convert.ToSingle(vertices[2]);

                parent.vertices.Add(new Point3D(x, y, z));
                parent.vertexFaces.Add(new List<int>());
                parent.normals.Add(new Normal(0, 0, 0));
                parent.countVertices++;
            }
        }

        public override void ParseFaces(Mesh parent, bool smooth)
        {
            int pastLastIndex = indexOfFaces + countFaces;
            int[] indexs = new int[5];
            MeshTriangle triangle;

            for (int i = indexOfFaces; i < pastLastIndex; i++)
            {
                string[] tokens = fileLines[i].Split(' ');
                int numVerts = Convert.ToInt32(tokens[0]);

                switch (numVerts)
                {
                    case 3: //Face is a triangle
                        for (int j = 1; j < 4; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        //Create new triangle
                        if (smooth)
                            triangle = new SmoothMeshTriangle(parent);
                        else
                            triangle = new FlatMeshTriangle(parent);
                        triangle.SetVertexIndices(indexs[1], indexs[2], indexs[3]);
                        //Update list of faces for given vertexs using current index
                        parent.vertexFaces[indexs[1]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[2]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[3]].Add(parent.countTriangles);
                        parent.countTriangles++;
                        parent.AddObject(triangle);
                        break;

                    case 4: //Face is a quad
                        for (int j = 1; j < 5; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        //Create new triangle
                        if (smooth)
                            triangle = new SmoothMeshTriangle(parent);
                        else
                            triangle = new FlatMeshTriangle(parent);
                        triangle.SetVertexIndices(indexs[1], indexs[2], indexs[3]);
                        //Update list of faces for given vertexs using current index
                        parent.vertexFaces[indexs[1]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[2]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[3]].Add(parent.countTriangles);
                        parent.countTriangles++;
                        parent.AddObject(triangle);
                        //Create new triangle
                        if (smooth)
                            triangle = new SmoothMeshTriangle(parent);
                        else
                            triangle = new FlatMeshTriangle(parent);
                        triangle.SetVertexIndices(indexs[3], indexs[4], indexs[1]);
                        //Update list of faces for given vertexs using current index
                        parent.vertexFaces[indexs[3]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[4]].Add(parent.countTriangles);
                        parent.vertexFaces[indexs[1]].Add(parent.countTriangles);
                        parent.countTriangles++;
                        parent.AddObject(triangle);
                        break;

                    default: //What the fuck kind of input are you feeding this poor parser?
                        throw new System.FormatException("Invalid token at line " + (i + 1) +
                            ": " + numVerts + "is not a valid number of vertices for a face.");
                }
            }
            CalculateNormals(parent);
        }

        private string GetOFFHeader()
        {
            int indexOfHeader = 0;
            //Traverse through the file, and find the first line that doesn't start with a comment
            for(int i = 1; i < fileLines.Length; i++)
            {
                //Tokenize line
                string[] tokens = fileLines[i].Split(' ');
                //Check if the first character of the tokenized line is #, if so this is a comment and should be skipped
                if(tokens[0].Substring(0,1).Equals("#"))
                {
                    //Do nothing
                }
                else
                {
                    //Found the header
                    indexOfHeader = i;
                    break;
                }
            }
            this.indexOfHeader = indexOfHeader;

            return fileLines[indexOfHeader];
        }
    }
}
