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
    class ImplicitTieFighter : RayMarchedImplicit
    {
        public override float evalF(Point3D p)
        {
            Vector3 one = p.Coordinates;
            Vector3 sqr = one * one;
            float t1 = Convert.ToSingle((sqr.X + sqr.Y + sqr.Z < 0.2));
            float t2 = (Convert.ToSingle(sqr.Y + sqr.Z < 0.08) * Convert.ToSingle(one.X < 0.4) * Convert.ToSingle(one.X > 0));
            //float t3 = Convert.ToSingle(Math.Pow(one.X,2+4*sqr.Y)<(1-Math.Abs(one.Z))
            return t1 + t2;
        }
    }
}
