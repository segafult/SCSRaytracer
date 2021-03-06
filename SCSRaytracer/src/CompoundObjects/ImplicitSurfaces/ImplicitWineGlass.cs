﻿using System;
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
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-3.0f);
            highBound = new Vector3(3.0f);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 9.0f;
            distanceMultiplier = 0.3f;
            triggerDistance = 0.1f;
        }
        public override float EvaluateImplicitFunction(Point3D p)
        {
            float xsqr = p.X * p.X;
            float ysqr = p.Y * p.Y;
            float ln = (float)Math.Log(p.Z + 3.2);
            return xsqr + ysqr - ln * ln - 0.02f;
        }
    }
}
