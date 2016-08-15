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
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-1.5f);
            highBound = new Vector3(1.5f);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 3.0f;
            distanceMultiplier = 0.1f;
            triggerDistance = 0.01f;
            r = 0.01f;
        }
        public override float EvaluateImplicitFunction(Point3D p)
        {
            Vector3 sqrd = p.Coordinates * p.Coordinates;
            Vector3 minusone = sqrd - new Vector3(1);
            float zeropoint8sqr = 0.8f * 0.8f;

            float t1 = sqrd.X + sqrd.Y - zeropoint8sqr;
            float t2 = sqrd.Y + sqrd.Z - zeropoint8sqr;
            float t3 = sqrd.Z + sqrd.X - zeropoint8sqr;

            return (t1 * t1 + minusone.Z * minusone.Z) * (t2 * t2 + minusone.X * minusone.X) * (t3 * t3 + minusone.Y * minusone.Y) - r;
        }
    }
}
