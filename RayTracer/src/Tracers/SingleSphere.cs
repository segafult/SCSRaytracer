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
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Placeholder raycaster with support for a single sphere
    /// </summary>
    public class SingleSphere : Tracer
    {
        private ShadeRec sr;
        public SingleSphere() : base()
        {
          
        }
        public SingleSphere(World wrld) : base(wrld)
        {
          
        }

        public override RGBColor trace_ray(Ray ray)
        {
            sr = new ShadeRec(world_pointer);
            //double t = 0.0;
            return GlobalVars.color_black;
            /*
            if (world_pointer.sphere.hit(ray, ref t, ref sr))
            {
                return (GlobalVars.color_red);
            }
            else { return (GlobalVars.color_black); }
            */
        }

        public override RGBColor trace_ray(Ray ray, int depth)
        {
            throw new NotImplementedException();
        }

        public override RGBColor trace_ray(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
