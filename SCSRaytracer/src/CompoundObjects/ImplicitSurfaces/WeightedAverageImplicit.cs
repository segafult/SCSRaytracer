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
        RayMarchedImplicit _implicit0, _implicit1;
        
        public RayMarchedImplicit Implicit0
        {
            set
            {
                _implicit0 = value;
            }
        }
        public RayMarchedImplicit Implicit1
        {
            set
            {
                _implicit1 = value;
            }
        }
        float parameter0, parameter1;
        public WeightedAverageImplicit()
        {
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-2.5f);
            highBound = new Vector3(2.5f);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 4.0f;
            distanceMultiplier = 0.1f;
            triggerDistance = 0.1f;
            parameter0 = 1.0f;
            parameter1 = 0.0f;
            _implicit0 = new ImplicitSphere();
            _implicit1 = new ImplicitDecocube();
        }

        public void CrossFade(float parameter)
        {
            parameter = FastMath.clamp(parameter, 0.0f, 1.0f);
            parameter1 = parameter;
            parameter0 = 1.0f - parameter;
        }

//        protected override float evalD(Point3D p, Vect3D d, ref float cur)
//        {
//            return base.evalD(p, d, ref cur);
 //       }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            return parameter0 * _implicit0.EvaluateImplicitFunction(p) + parameter1 * _implicit1.EvaluateImplicitFunction(p);
        }
    }
}
