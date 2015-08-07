using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    abstract class MeshLoader
    {
        protected BoundingBox bb;

        abstract public bool openFile(string filename);
        abstract public List<Point3D> parseVertices();
        abstract public List<Triangle> parseFaces(List<Point3D> verts);
        virtual public List<Point2D> parseUV() { return null; }
        virtual public BoundingBox getBoundingBox() { return bb; }
    }
}
