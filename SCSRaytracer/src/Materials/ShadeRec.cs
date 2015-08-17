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

namespace RayTracer
{
    /// <summary>
    /// Convenient bundle for passing information between shader subroutines.
    /// </summary>
    struct ShadeRec
    {
        public bool hit_an_object; //Whether ray hit an object
        public Material obj_material; //Material reference for shading
        public Point3D hit_point; //Hit point (world coordinates)
        public Point3D hit_point_local; //Hit point in local coordinates for UV mapping
        public Normal normal; //Normal at hit point
        public RGBColor color; //Color at hit point
        public float t; //Tmin (minimum distance) for a given ray intersection.
        public World w;

        public Ray ray; //Ray for specular highlights
        public int depth; //Recursion depth (reflection)
        public Vect3D dir; //Area lighting

        public float u; //UV coordinates
        public float v;

        //Constructor
        public ShadeRec(World worldRef)
        {
            hit_an_object = false;
            obj_material = null;
            depth = 0;
            w = worldRef;
            hit_point = new Point3D(0, 0, 0);
            hit_point_local = new Point3D(0, 0, 0);
            normal = new Normal(0, 0, 0);
            color = new RGBColor(0, 0, 0);
            t = GlobalVars.kHugeValue;
            ray = new Ray(new Point3D(0, 0, 0), new Vect3D(0, 0, 0));
            dir = new Vect3D(0, 0, 0);
            u = 0;
            v = 0;
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
            t = sr.t;
            u = sr.u;
            v = sr.v;
        }
    }
}
