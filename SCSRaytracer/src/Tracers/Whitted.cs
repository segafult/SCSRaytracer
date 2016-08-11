//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    /// <summary>
    /// A Whitted style recursive raycaster
    /// </summary>
    sealed class Whitted : Tracer
    {
        /// <summary>
        /// Parameterized constructor, hands off to base class
        /// </summary>
        /// <param name="worldPointer">Pointer to current world</param>
        public Whitted(World worldPointer) : base(worldPointer)
        {
            
        }

        public override RGBColor TraceRay(Ray ray)
        {
            throw new NotImplementedException();
        }

        public override RGBColor TraceRay(Ray ray, int depth)
        {
            // If depth exceeded maximum depth, no hits will occurr, and pixel will be black
            if(depth > worldPointer.CurrentViewPlane.MaximumRenderDepth)
            {
                return (GlobalVars.COLOR_BLACK);
            }
            // Otherwise fetch shader info for current ray, assuming a hit occurs
            else
            {
                ShadeRec shadeRec = worldPointer.HitObjects(ray);

                if(shadeRec.HitAnObject)
                {
                    shadeRec.RecursionDepth = depth;
                    shadeRec.Ray = ray;
                    // apply shading and return color
                    return (shadeRec.ObjectMaterial.Shade(shadeRec));
                }
                else
                {
                    // return background color if no hits
                    return worldPointer.CurrentBackgroundColor;
                }
            }
        }
        public override RGBColor TraceRay(Ray ray, float tMin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
