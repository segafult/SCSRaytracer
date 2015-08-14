//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;

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
