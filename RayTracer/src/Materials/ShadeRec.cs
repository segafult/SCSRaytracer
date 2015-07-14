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
    /// Convenient bundle for passing information between shader subroutines.
    /// </summary>
    public class ShadeRec
    {
        public bool hit_an_object; //Whether ray hit an object
        public Material obj_material; //Material reference for shading
        public Point3D hit_point; //Hit point (world coordinates)
        public Point3D hit_point_local; //Hit point in local coordinates for UV mapping
        public Normal normal; //Normal at hit point
        public RGBColor color; //Color at hit point
        public double t; //Tmin (minimum distance) for a given ray intersection.
        public World w;

        public Ray ray; //Ray for specular highlights
        public int depth; //Recursion depth (reflection)
        public Vect3D dir; //Area lighting

        //Constructor
        public ShadeRec(World worldRef)
        {
            hit_an_object = false;
            obj_material = null;
            depth = 0;
            w = worldRef;
        }
        //Copy constructor
        public ShadeRec(ShadeRec sr)
        {
            hit_an_object = sr.hit_an_object;
            obj_material = sr.obj_material;
            hit_point = sr.hit_point;
            hit_point_local = sr.hit_point_local;
            normal = sr.normal;
            color = sr.color;
            w = sr.w;
            ray = sr.ray;
            depth = sr.depth;
            dir = sr.dir;
        }
    }
}
