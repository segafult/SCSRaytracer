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
        protected World world_pointer;

        /// <summary>
        /// Default constructor with null world pointer
        /// </summary>
        public Tracer()
        {
            world_pointer = null;
        }
        /// <summary>
        /// Constructor for already existing world
        /// </summary>
        /// <param name="wrld">Fully constructed current world</param>
        public Tracer(World wrld)
        {
            world_pointer = wrld;
        }

        /// <summary>
        /// Simple raytracing, no recursion, for shadowcasting
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor trace_ray(Ray ray);

        /// <summary>
        /// Recursive raytracing with whitted algorithm
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <param name="depth">Current depth</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor trace_ray(Ray ray, int depth);

        /// <summary>
        /// Raytracing with specified current minimum hit distance for ray
        /// </summary>
        /// <param name="ray">Ray to trace</param>
        /// <param name="tmin">Current tmin</param>
        /// <param name="depth">Current depth</param>
        /// <returns>Color at pixel</returns>
        public abstract RGBColor trace_ray(Ray ray, float tmin, int depth);
    }
}
