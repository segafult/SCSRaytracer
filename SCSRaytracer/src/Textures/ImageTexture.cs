//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    sealed class ImageTexture : Texture
    {
        Image _image;

        public Image Image
        {
            set
            {
                _image = value;
            }
        }
        //public void set_image(Image img_arg) { img = img_arg; }

        public override RGBColor GetColor(ShadeRec sr)
        {
            float u;
            float v;

            //Use provided UV coordinates if no map type defined
            if (_mapType == null)
            {
                u = sr.U;
                v = sr.U;
            }
            else
            {
                Point2D uv = _mapType.GetUV(sr.HitPointLocal);
                u = uv.coords.X;
                v = uv.coords.Y;
            }

            return _image.GetColorAtUV(u, v, true);
        }
    }
}
