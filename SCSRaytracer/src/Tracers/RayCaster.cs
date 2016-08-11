//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    /// <summary>
    /// A tracer that traces a ray using the ray casting algorithm.
    /// </summary>
    sealed class RayCaster : Tracer
    {
        public RayCaster(World w)
        {
            worldPointer = w;
        }

        /// <summary>
        /// Returns color of a ray traced in the scene.
        /// </summary>
        /// <param name="ray">Ray for tracing</param>
        /// <returns>Color of the object intersected, or the background color if no intersection occurred.</returns>
        public override RGBColor TraceRay(Ray ray)
        {
            ShadeRec shadeRec = new ShadeRec(worldPointer.HitObjects(ray));

            if(shadeRec.HitAnObject)
            {
                //Console.Write("hit");
                //return new RGBColor(1.0, 0, 0);
                shadeRec.Ray = ray; //Store information for specular highlight.
                return (shadeRec.ObjectMaterial.Shade(shadeRec)); //Call shader function for object material.
            }
            else { return worldPointer.CurrentBackgroundColor; } //No need to call shader function if no intersection occurred.
        }

        public override RGBColor TraceRay(Ray ray, float tMin, int depth)
        {
            throw new NotImplementedException();
        }
        public override RGBColor TraceRay(Ray ray, int depth)
        {
            return TraceRay(ray);
        }
    }
}
