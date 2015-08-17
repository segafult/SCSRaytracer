//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    abstract class Material
    {
        public string id;
        public abstract RGBColor shade(ShadeRec sr);
    }
}
