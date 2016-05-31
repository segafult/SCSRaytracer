//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    class DebugCheckerboard : PhongShader
    {

        public override RGBColor shade(ShadeRec sr)
        {
            RGBColor multiplar = base.shade(sr);
            float scalefactor;
            if(Math.Abs(sr.hit_point.X) % 50 < 25 && Math.Abs(sr.hit_point.Z) % 50 > 25)
            {
                scalefactor = 0.1f;
            }
            else
            {
                scalefactor = 1.0f;
            }
            return scalefactor * multiplar;
        }
    }
}
