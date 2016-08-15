//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitSphere : RayMarchedImplicit
    {
        private float r;
        private Vector3 disp; //The center point of the sphere

        public ImplicitSphere()
        {
            r = 1.0f;
            disp = new Vector3(0, 0, 0);
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-2*r);
            highBound = new Vector3(2*r);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 5.0f;
            distanceMultiplier = 0.3f;
            triggerDistance = 0.1f;
        }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            Vector3 pretranslation = p.Coordinates - disp;
            Vector3 tmp = pretranslation * pretranslation;
            return tmp.X + tmp.Y + tmp.Z - r * r;
        }

        protected override float EvaluateDistanceFunction(Point3D p, Vect3D d, ref float cur)
        {
            //Translate the point
            cur = EvaluateImplicitFunction(p);
            return (p.Coordinates - disp).Length() - r;
        }
    }
}
