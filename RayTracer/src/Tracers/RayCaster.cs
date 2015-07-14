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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// A tracer that traces a ray using the ray casting algorithm.
    /// </summary>
    public class RayCaster : Tracer
    {
        public RayCaster(World w)
        {
            world_pointer = w;
        }

        /// <summary>
        /// Returns color of a ray traced in the scene.
        /// </summary>
        /// <param name="ray">Ray for tracing</param>
        /// <returns>Color of the object intersected, or the background color if no intersection occurred.</returns>
        public override RGBColor trace_ray(Ray ray)
        {
            ShadeRec sr = new ShadeRec(world_pointer.hit_objects(ray));

            if(sr.hit_an_object)
            {
                //Console.Write("hit");
                //return new RGBColor(1.0, 0, 0);
                sr.ray = ray; //Store information for specular highlight.
                return (sr.obj_material.shade(sr)); //Call shader function for object material.
            }
            else { return world_pointer.bg_color; } //No need to call shader function if no intersection occurred.
        }

        public override RGBColor trace_ray(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
        public override RGBColor trace_ray(Ray ray, int depth)
        {
            return trace_ray(ray);
        }
    }
}
