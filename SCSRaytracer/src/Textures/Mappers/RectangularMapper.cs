//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
// 

namespace SCSRaytracer
{
    class RectangularMapper : Mapper
    {
        public override Point2D GetUV(Point3D hitPoint)
        {
            //simple rectangular mapper maps to unit rectangle in xz plane
            return new Point2D((hitPoint.Z + 1.0f) / 2.0f, (hitPoint.X + 1.0f) / 2.0f);
        }
    }
}
