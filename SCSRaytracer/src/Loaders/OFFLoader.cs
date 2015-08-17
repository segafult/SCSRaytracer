//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;

namespace RayTracer
{
    class OFFLoader : MeshLoader
    {
        string[] fileLines;

        int index_of_header;
        int index_of_verts;
        int index_of_faces;
        
        int numverts;
        int numfaces;
        

        public override bool openFile(string filename)
        {
            //if(System.IO.File.Exists(filename))
            //{
                fileLines = System.IO.File.ReadAllLines(filename);

                //To be a valid .off file, must start with 'OFF'
                if (fileLines[0].Equals("OFF"))
                {
                    string header = this.getOFFHeader();

                    //Tokenize header and extract file info
                    string[] header_data = header.Split(' ');
                    numverts = Convert.ToInt32(header_data[0]);
                    numfaces = Convert.ToInt32(header_data[1]);

                    //Set the indices of data based on extracted header info
                    index_of_verts = index_of_header + 1;
                    index_of_faces = index_of_verts + numverts;

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

        public override void parseVertices(Mesh parent)
        {
            int past_last_index = index_of_verts + numverts;

            for(int i = index_of_verts;i<past_last_index;i++)
            {
                //Tokenize each line...
                string[] vert_data = fileLines[i].Split(' ');

                float x = Convert.ToSingle(vert_data[0]);
                float y = Convert.ToSingle(vert_data[1]);
                float z = Convert.ToSingle(vert_data[2]);

                parent.vertices.Add(new Point3D(x, y, z));
                parent.vertex_faces.Add(new List<int>());
                parent.normals.Add(new Normal(0,0,0));
                parent.num_verts++;
            }
        }

        public override void parseFaces(Mesh parent, bool smooth)
        {
            int past_last_index = index_of_faces + numfaces;
            int[] indexs = new int[5];
            MeshTriangle tri_ptr;

            for(int i = index_of_faces; i < past_last_index; i++)
            {
                string[] tokens = fileLines[i].Split(' ');
                int numVerts = Convert.ToInt32(tokens[0]);

                switch(numVerts)
                {
                    case 3: //Face is a triangle
                        for(int j = 1; j < 4; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        //Create new triangle
                        if (smooth)
                            tri_ptr = new SmoothMeshTriangle(parent);
                        else
                            tri_ptr = new FlatMeshTriangle(parent);
                        tri_ptr.setVertexIndices(indexs[1], indexs[2], indexs[3]);
                        //Update list of faces for given vertexs using current index
                        parent.vertex_faces[indexs[1]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[2]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[3]].Add(parent.num_triangles);
                        parent.num_triangles++;
                        parent.add_object(tri_ptr);
                        break;

                    case 4: //Face is a quad
                        for(int j = 1; j < 5; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        //Create new triangle
                        if (smooth)
                            tri_ptr = new SmoothMeshTriangle(parent);
                        else
                            tri_ptr = new FlatMeshTriangle(parent);
                        tri_ptr.setVertexIndices(indexs[1], indexs[2], indexs[3]);
                        //Update list of faces for given vertexs using current index
                        parent.vertex_faces[indexs[1]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[2]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[3]].Add(parent.num_triangles);
                        parent.num_triangles++;
                        parent.add_object(tri_ptr);
                        //Create new triangle
                        if (smooth)
                            tri_ptr = new SmoothMeshTriangle(parent);
                        else
                            tri_ptr = new FlatMeshTriangle(parent);
                        tri_ptr.setVertexIndices(indexs[3], indexs[4], indexs[1]);
                        //Update list of faces for given vertexs using current index
                        parent.vertex_faces[indexs[3]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[4]].Add(parent.num_triangles);
                        parent.vertex_faces[indexs[1]].Add(parent.num_triangles);
                        parent.num_triangles++;
                        parent.add_object(tri_ptr);
                        break;

                    default: //What the fuck kind of input are you feeding this poor parser?
                        throw new System.FormatException("Invalid token at line " + (i + 1) +
                            ": " + numVerts + "is not a valid number of vertices for a face.");
                }  
            }
            calculateNormals(parent);
        }

        private string getOFFHeader()
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
            index_of_header = indexOfHeader;

            return fileLines[indexOfHeader];
        }
    }
}
