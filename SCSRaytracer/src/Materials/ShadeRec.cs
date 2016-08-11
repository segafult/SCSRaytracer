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
    class ShadeRec
    {
        public bool HitAnObject; //Whether ray hit an object
        public Material ObjectMaterial; //Material reference for shading
        public Point3D HitPoint; //Hit point (world coordinates)
        public Point3D HitPointLocal; //Hit point in local coordinates for UV mapping
        public Normal Normal; //Normal at hit point
        public RGBColor Color; //Color at hit point
        public float TMinimum; //Tmin (minimum distance) for a given ray intersection.
        public World WorldPointer;

        public Ray Ray; //Ray for specular highlights
        public int RecursionDepth; //Recursion depth (reflection)
        public Vect3D Direction; //Area lighting

        public float U; //UV coordinates
        public float V;

        //Constructor
        public ShadeRec(World worldRef)
        {
            HitAnObject = false;
            ObjectMaterial = null;
            RecursionDepth = 0;
            WorldPointer = worldRef;
            HitPoint = new Point3D(0, 0, 0);
            HitPointLocal = new Point3D(0, 0, 0);
            Normal = new Normal(0, 0, 0);
            Color = new RGBColor(0, 0, 0);
            TMinimum = GlobalVars.K_HUGE_VALUE;
            Ray = new Ray(new Point3D(0, 0, 0), new Vect3D(0, 0, 0));
            Direction = new Vect3D(0, 0, 0);
            U = 0;
            V = 0;
        }
        //Copy constructor
        public ShadeRec(ShadeRec shadeRec)
        {
            HitAnObject = shadeRec.HitAnObject;
            ObjectMaterial = shadeRec.ObjectMaterial;
            HitPoint = shadeRec.HitPoint;
            HitPointLocal = shadeRec.HitPointLocal;
            Normal = shadeRec.Normal;
            Color = shadeRec.Color;
            WorldPointer = shadeRec.WorldPointer;
            Ray = shadeRec.Ray;
            RecursionDepth = shadeRec.RecursionDepth;
            Direction = shadeRec.Direction;
            TMinimum = shadeRec.TMinimum;
            U = shadeRec.U;
            V = shadeRec.V;
        }
    }
}
