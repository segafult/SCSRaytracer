//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    abstract class Material
    {
        public string id;
        public abstract RGBColor shade(ShadeRec sr);
    }
}
