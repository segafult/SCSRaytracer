//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitDecocube : RayMarchedImplicit
    {
        private float r;
        public ImplicitDecocube()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-1.5f);
            highbound = new Vector3(1.5f);
            min_step = 1.0e-5f;
            max_step = 4.0f;
            dist_mult = 0.1f;
            trigger_dist = 0.1f;
            r = 0.1f;
        }
        public override float evalF(Point3D p)
        {
            Vector3 sqrd = p.coords * p.coords;
            Vector3 minusone = sqrd - new Vector3(1);
            float zeropoint8sqr = 0.8f * 0.8f;

            float t1 = sqrd.X + sqrd.Y - zeropoint8sqr;
            float t2 = sqrd.Y + sqrd.Z - zeropoint8sqr;
            float t3 = sqrd.Z + sqrd.X - zeropoint8sqr;

            return (t1 * t1 + minusone.Z * minusone.Z) * (t2 * t2 + minusone.X * minusone.X) * (t3 * t3 + minusone.Y * minusone.Y) - r;
        }
    }
}
