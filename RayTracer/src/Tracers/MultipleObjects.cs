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
    /// Temporary placeholder raycaster object. To be replaced by more feature complete raycaster.
    /// </summary>
    public class MultipleObjects : Tracer
    {
        public MultipleObjects(World wrld)
        {
            world_pointer = wrld;
        }
        public override RGBColor trace_ray(Ray ray)
        {

            ShadeRec sr = world_pointer.hit_barebones_objects(ray);
            if(sr.hit_an_object)
            {
                sr.normal.normalize();
                //Perform lighting calculations
                //Vect3D light_direction = world_pointer.lightList[0].getLightDirection(sr.hit_point);
                //RGBColor tempcolor = new RGBColor(0, 0, 0);
                //tempcolor = tempcolor + sr.color * (Math.Pow((light_direction * sr.normal),1.5) + 1);
                //sr.color = tempcolor;
                return sr.color;
            }
            else
            {
                return world_pointer.bg_color;
            }
        }

        public override RGBColor trace_ray(Ray ray, int depth)
        {
            return this.trace_ray(ray);
        }
        public override RGBColor trace_ray(Ray ray, float tmin, int depth )
        {
            return this.trace_ray(ray);
        }
    }
}
