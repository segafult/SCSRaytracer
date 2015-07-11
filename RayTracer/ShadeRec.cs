using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class ShadeRec
    {
        public bool hit_an_object; //Whether ray hit an object
        public Point3D hit_point; //Hit point (world coordinates)
        public Normal normal; //Normal at hit point
        public RGBColor color; //Color at hit point
        public World w;

        //Constructor
        public ShadeRec(World worldRef)
        {
            w = worldRef;
        }
    }
}
