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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class ImageTexture : Texture
    {
        Image img;

        public void set_image(Image img_arg) { img = img_arg; }

        public override RGBColor getColor(ShadeRec sr)
        {
            double u;
            double v;

            //Use provided UV coordinates if no map type defined
            if (maptype == null)
            {
                u = sr.u;
                v = sr.u;
            }
            else
            {
                Point2D uv = maptype.get_uv(sr.hit_point_local);
                u = uv.x;
                v = uv.y;
            }

            return img.get_color_at_uv(u, v, true);
        }
    }
}
