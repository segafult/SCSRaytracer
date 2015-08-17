//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    class Point2D
    {
        public float x;
        public float y;

        public Point2D()
        {
            x = 0.0f;
            y = 0.0f;
        }
        public Point2D(float xcoord, float ycoord)
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
