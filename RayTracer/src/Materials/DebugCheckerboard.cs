using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class DebugCheckerboard : PhongShader
    {

        public override RGBColor shade(ShadeRec sr)
        {
            RGBColor multiplar = base.shade(sr);
            double scalefactor;
            if(Math.Abs(sr.hit_point.xcoord) % 50 < 25 && Math.Abs(sr.hit_point.zcoord) % 50 > 25)
            {
                scalefactor = 0.1;
            }
            else
            {
                scalefactor = 1.0;
            }
            return scalefactor * multiplar;
        }
    }
}
