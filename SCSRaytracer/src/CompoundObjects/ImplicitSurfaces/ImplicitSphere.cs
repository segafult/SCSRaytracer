//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace RayTracer
{
    class ImplicitSphere : RayMarchedImplicit
    {
        private float r;
        private Vector3 disp; //The center point of the sphere

        public ImplicitSphere()
        {
            r = 1.0f;
            disp = new Vector3(1, 0, 0);
            bbox = new BoundingBox();
            lowbound = new Vector3(-2*r);
            highbound = new Vector3(2*r);
            min_step = 1.0e-5f;
            max_step = 5.0f;
            dist_mult = 0.3f;
            trigger_dist = 0.1f;
        }

        protected override float evalF(Point3D p)
        {
            Vector3 pretranslation = p.coords - disp;
            Vector3 tmp = pretranslation * pretranslation;
            return tmp.X + tmp.Y + tmp.Z - r * r;
        }

        protected override float evalD(Point3D p, Vect3D d, ref float cur)
        {
            //Translate the point
            cur = evalF(p);
            return (p.coords - disp).Length() - r;
        }
    }
}
