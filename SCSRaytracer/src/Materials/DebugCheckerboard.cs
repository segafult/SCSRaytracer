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

        public override RGBColor Shade(ShadeRec sr)
        {
            RGBColor multiplar = base.Shade(sr);
            float scalefactor;
            if(Math.Abs(sr.HitPoint.X) % 50 < 25 && Math.Abs(sr.HitPoint.Z) % 50 > 25)
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
