//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
// 

namespace SCSRaytracer
{
    class RectangularMapper : Mapper
    {
        public override Point2D get_uv(Point3D hit_point)
        {
            //simple rectangular mapper maps to unit rectangle in xz plane
            return new Point2D((hit_point.Z + 1.0f) / 2.0f, (hit_point.X + 1.0f) / 2.0f);
        }
    }
}
