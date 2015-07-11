using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
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
                Vect3D light_direction = world_pointer.lightList[0].getLightDirection(sr.hit_point);
                RGBColor tempcolor = new RGBColor(0, 0, 0);
                tempcolor = tempcolor + sr.color * (Math.Pow((light_direction * sr.normal),1.5) + 1);
                sr.color = tempcolor;
                return sr.color;
            }
            else
            {
                return world_pointer.bg_color;
            }
        }
    }
}
