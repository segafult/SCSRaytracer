//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    /// <summary>
    /// Template class for all other tracers
    /// </summary>
    abstract class Tracer
    {
        protected World worldPointer;

        /// <summary>
        /// Default constructor with null world pointer
        /// </summary>
        public Tracer()
        {
            worldPointer = null;
        }
        /// <summary>
        /// Constructor for already existing world
        /// </summary>
        /// <param name="world">Fully constructed current world</param>
        public Tracer(World world)
        {
            worldPointer = world;
        }

        /// <summary>
        /// Simple raytracing, no recursion, for shadowcasting
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor TraceRay(Ray ray);

        /// <summary>
        /// Recursive raytracing with whitted algorithm
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <param name="depth">Current depth</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor TraceRay(Ray ray, int depth);

        /// <summary>
        /// Raytracing with specified current minimum hit distance for ray
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <param name="tMin">Current tmin</param>
        /// <param name="depth">Current depth</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor TraceRay(Ray ray, float tMin, int depth);
    }
}
