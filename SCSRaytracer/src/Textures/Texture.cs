//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    abstract class Texture
    {

        protected Mapper maptype = null;

        public Texture()
        {

        }

        public void setMapper(Mapper map) { maptype = map; }

        virtual public RGBColor getColor(ShadeRec sr)
        {
            return new RGBColor(1, 1, 1);
        }
    }
}
