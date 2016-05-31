//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    sealed class ImageTexture : Texture
    {
        Image img;

        public void set_image(Image img_arg) { img = img_arg; }

        public override RGBColor getColor(ShadeRec sr)
        {
            float u;
            float v;

            //Use provided UV coordinates if no map type defined
            if (maptype == null)
            {
                u = sr.u;
                v = sr.u;
            }
            else
            {
                Point2D uv = maptype.get_uv(sr.hit_point_local);
                u = uv.coords.X;
                v = uv.coords.Y;
            }

            return img.GetColorAtUV(u, v, true);
        }
    }
}
