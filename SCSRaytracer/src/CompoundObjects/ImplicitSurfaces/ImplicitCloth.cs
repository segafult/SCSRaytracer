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
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-30,-8,-30);
            highBound = new Vector3(30, 8, 30);
            minimumRaymarchStep = 1.0e-4f;
            maximumRaymarchStep = 10.0f;
            distanceMultiplier = 0.1f;
            triggerDistance = 0.1f;
            w = (float)Math.PI;
            RECURSIONDEPTH = 15;
        }

        public void setW(float w_arg) { w = w_arg; }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            return p.Y - (float)(0.5 * Math.Sin(p.X + 3 * w)) - 0.1f * (float)(((1 + 0.2 * Math.Sin(p.X * p.Z))) * Math.Cos(p.Z + 3 * w));
        }
    }
}
