//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;

namespace RayTracer
{
    /// <summary>
    /// A Whitted style recursive raycaster
    /// </summary>
    class Whitted : Tracer
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
