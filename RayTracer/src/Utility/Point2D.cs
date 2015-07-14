using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Point2D
    {
        public double x;
        public double y;

        public Point2D()
        {
            x = 0.0;
            y = 0.0;
        }
        public Point2D(double xcoord, double ycoord)
        {
            x = xcoord;
            y = ycoord;
        }
        public Point2D(Point2D p)
        {
            x = p.x;
            y = p.y;
        }
    }
}
