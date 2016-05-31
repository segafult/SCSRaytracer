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

        public override RGBColor trace_ray(Ray ray)
        {
            throw new NotImplementedException();
        }

        public override RGBColor trace_ray(Ray ray, int depth)
        {
            // If depth exceeded maximum depth, no hits will occurr, and pixel will be black
            if(depth > world_pointer.CurrentViewPlane.MaximumRenderDepth)
            {
                return (GlobalVars.COLOR_BLACK);
            }
            // Otherwise fetch shader info for current ray, assuming a hit occurs
            else
            {
                ShadeRec sr = world_pointer.HitObjects(ray);

                if(sr.hit_an_object)
                {
                    sr.depth = depth;
                    sr.ray = ray;
                    // apply shading and return color
                    return (sr.obj_material.shade(sr));
                }
                else
                {
                    // return background color if no hits
                    return world_pointer.CurrentBackgroundColor;
                }
            }
        }
        public override RGBColor trace_ray(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
