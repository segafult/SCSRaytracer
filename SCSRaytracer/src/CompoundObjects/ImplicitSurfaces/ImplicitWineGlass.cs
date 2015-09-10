using System;
using System.Collections.Generic;
using System.Linq;
//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Text;
using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitWineGlass : RayMarchedImplicit
    {
        public ImplicitWineGlass()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-3.0f);
            highbound = new Vector3(3.0f);
            min_step = 1.0e-5f;
            max_step = 9.0f;
            dist_mult = 0.3f;
            trigger_dist = 0.1f;
        }
        public override float evalF(Point3D p)
        {
            float xsqr = p.coords.X * p.coords.X;
            float ysqr = p.coords.Y * p.coords.Y;
            float ln = (float)Math.Log(p.coords.Z + 3.2);
            return xsqr + ysqr - ln * ln - 0.02f;
        }
    }
}
