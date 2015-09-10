using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace SCSRaytracer
{
    class WeightedAverageImplicit : RayMarchedImplicit
    {
        RayMarchedImplicit i0, i1;
        float p0, p1;
        public WeightedAverageImplicit()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-2.5f);
            highbound = new Vector3(2.5f);
            min_step = 1.0e-5f;
            max_step = 4.0f;
            dist_mult = 0.1f;
            trigger_dist = 0.1f;
            p0 = 1.0f;
            p1 = 0.0f;
            i0 = new ImplicitSphere();
            i1 = new ImplicitDecocube();
        }

        public void crossFade(float p_arg)
        {
            p_arg = FastMath.clamp(p_arg, 0.0f, 1.0f);
            p1 = p_arg;
            p0 = 1.0f - p_arg;
        }

//        protected override float evalD(Point3D p, Vect3D d, ref float cur)
//        {
//            return base.evalD(p, d, ref cur);
 //       }

        public override float evalF(Point3D p)
        {
            return p0 * i0.evalF(p) + p1 * i1.evalF(p);
        }
    }
}
