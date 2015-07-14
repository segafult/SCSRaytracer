using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Placeholder raycaster with support for a single sphere
    /// </summary>
    public class SingleSphere : Tracer
    {
        private ShadeRec sr;
        public SingleSphere() : base()
        {
          
        }
        public SingleSphere(World wrld) : base(wrld)
        {
          
        }

        public override RGBColor trace_ray(Ray ray)
        {
            sr = new ShadeRec(world_pointer);
            double t = 0.0;
            return GlobalVars.color_black;
            /*
            if (world_pointer.sphere.hit(ray, ref t, ref sr))
            {
                return (GlobalVars.color_red);
            }
            else { return (GlobalVars.color_black); }
            */
        }

        public override RGBColor trace_ray(Ray ray, int depth)
        {
            throw new NotImplementedException();
        }

        public override RGBColor trace_ray(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
