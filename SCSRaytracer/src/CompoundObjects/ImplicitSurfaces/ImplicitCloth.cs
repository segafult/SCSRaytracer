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
    class ImplicitCloth : RayMarchedImplicit
    {
        protected float w;
        public ImplicitCloth()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-30,-8,-30);
            highbound = new Vector3(30, 8, 30);
            min_step = 1.0e-4f;
            max_step = 10.0f;
            dist_mult = 0.1f;
            trigger_dist = 0.1f;
            w = (float)Math.PI;
            RECURSIONDEPTH = 15;
        }

        public void setW(float w_arg) { w = w_arg; }

        public override float evalF(Point3D p)
        {
            return p.Y - (float)(0.5 * Math.Sin(p.X + 3 * w)) - 0.1f * (float)(((1 + 0.2 * Math.Sin(p.X * p.Z))) * Math.Cos(p.Z + 3 * w));
        }
    }
}
