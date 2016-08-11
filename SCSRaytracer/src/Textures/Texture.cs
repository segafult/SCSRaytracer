//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    abstract class Texture
    {

        protected Mapper _mapType = null;

        public Mapper MapType
        {
            set
            {
                _mapType = value;
            }
        }

        public Texture()
        {

        }

        //public void setMapper(Mapper map) { _mapType = map; }

        virtual public RGBColor GetColor(ShadeRec sr)
        {
            return new RGBColor(1, 1, 1);
        }
    }
}
