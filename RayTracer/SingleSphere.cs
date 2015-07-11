using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class SingleSphere : Tracer
    {
        private ShadeRec sr;
        public SingleSphere() : base()
        {
          
        }
        public SingleSphere(World wrld) : base(wrld)
        {
          
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}
