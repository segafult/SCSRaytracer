//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    /// <summary>
    /// Template class for all other tracers
    /// </summary>
    abstract class Tracer
    {
        protected World world_pointer;

        public Tracer()
        {
            world_pointer = null;
        }
        public Tracer(World wrld)
        {
            world_pointer = wrld;
        }

        public abstract RGBColor trace_ray(Ray ray);
        public abstract RGBColor trace_ray(Ray ray, int depth);
        public abstract RGBColor trace_ray(Ray ray, float tmin, int depth);
    }
}
