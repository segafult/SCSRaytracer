//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace RayTracer
{
    /// <summary>
    /// A Whitted style recursive raycaster
    /// </summary>
    sealed class Whitted : Tracer
    {
        public Whitted(World wptr) : base(wptr)
        {
            
        }
        public override RGBColor trace_ray(Ray ray)
        {
            throw new NotImplementedException();
        }
        public override RGBColor trace_ray(Ray ray, int depth)
        {
            if(depth > world_pointer.vp.maxDepth)
            {
                return (GlobalVars.color_black);
            }
            else
            {
                ShadeRec sr = world_pointer.hit_objects(ray);

                if(sr.hit_an_object)
                {
                    sr.depth = depth;
                    sr.ray = ray;

                    return (sr.obj_material.shade(sr));
                }
                else
                {
                    return world_pointer.bg_color;
                }
            }
        }
        public override RGBColor trace_ray(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
