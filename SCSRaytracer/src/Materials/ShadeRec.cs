//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
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
            t = GlobalVars.K_HUGE_VALUE;
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
