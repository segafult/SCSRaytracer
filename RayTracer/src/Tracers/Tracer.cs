using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Template class for all other tracers
    /// </summary>
    public abstract class Tracer
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
