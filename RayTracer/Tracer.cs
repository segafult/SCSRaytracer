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
    public class Tracer
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

        public virtual RGBColor trace_ray(Ray ray)
        {
            return GlobalVars.color_black;
        }
    }
}
