//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace RayTracer
{
    struct Point2D
    {
        public Vector2 coords; 

        public Point2D(float xcoord, float ycoord)
        {
            coords = new Vector2(xcoord, ycoord);
        }
        public Point2D(Point2D p)
        {
            coords = p.coords;
        }
    }
}
