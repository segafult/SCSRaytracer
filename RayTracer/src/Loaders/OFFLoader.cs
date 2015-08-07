using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if(System.IO.File.Exists(filename))
            {
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
            }
            else
            {
                return false;
            }
        }

        public override List<Point3D> parseVertices()
        {
            List<Point3D> toReturn = new List<Point3D>();
            int past_last_index = index_of_verts + numverts;

            //Values for computing bounding box
            double xmin = GlobalVars.kHugeValue;
            double xmax = -GlobalVars.kHugeValue;
            double ymin = GlobalVars.kHugeValue;
            double ymax = -GlobalVars.kHugeValue;
            double zmin = GlobalVars.kHugeValue;
            double zmax = -GlobalVars.kHugeValue;

            for(int i = index_of_verts;i<past_last_index;i++)
            {
                //Tokenize each line...
                string[] vert_data = fileLines[i].Split(' ');

                double x = Convert.ToDouble(vert_data[0]);
                xmin = (x < xmin) ? x : xmin;
                xmax = (x > xmax) ? x : xmax;

                double y = Convert.ToDouble(vert_data[1]);
                ymin = (y < ymin) ? y : ymin;
                ymax = (y > ymax) ? y : ymax;

                double z = Convert.ToDouble(vert_data[2]);
                zmin = (z < zmin) ? z : zmin;
                zmax = (z > zmax) ? z : zmax;

                toReturn.Add(new Point3D(x, y, z));
            }

            //Construct bounding box
            bb = new BoundingBox(xmin, xmax, ymin, ymax, zmin, zmax);

            return toReturn;
        }

        public override List<Triangle> parseFaces(List<Point3D> verts)
        {
            List<Triangle> toReturn = new List<Triangle>();
            int past_last_index = index_of_faces + numfaces;
            int[] indexs = new int[5];

            for(int i = index_of_faces; i < past_last_index; i++)
            {
                string[] tokens = fileLines[i].Split(' ');
                int numVerts = Convert.ToInt32(tokens[0]);

                //Described face is a triangle
                switch(numVerts)
                {
                    case 3: //Face is a triangle
                        for(int j = 1; j < 4; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        toReturn.Add(new Triangle(verts[indexs[1]], verts[indexs[2]], verts[indexs[3]]));
                        break;

                    case 4: //Face is a quad
                        for(int j = 1; j < 5; j++)
                        {
                            indexs[j] = Convert.ToInt32(tokens[j]);
                        }
                        toReturn.Add(new Triangle(verts[indexs[1]], verts[indexs[2]], verts[indexs[3]]));
                        toReturn.Add(new Triangle(verts[indexs[3]], verts[indexs[4]], verts[indexs[1]]));
                        break;

                    default: //What the fuck kind of input are you feeding this poor parser?
                        throw new System.FormatException("Invalid token at line " + (i + 1) +
                            ": " + numVerts + "is not a valid number of vertices for a face.");
                }
            }

            return toReturn;
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
