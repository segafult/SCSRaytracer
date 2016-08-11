//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    abstract class Mapper
    {
        protected readonly float INVPI = 1.0f / (float)Math.PI;
        protected readonly float INVTWOPI = 1.0f / (2.0f * (float)Math.PI);


        abstract public Point2D GetUV(Point3D hitPoint);
    }
}
