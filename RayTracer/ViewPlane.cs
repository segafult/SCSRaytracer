using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class ViewPlane
    {
        public int hres;
        public int vres;
        public float s;
        public float gamma;
        public float inv_gamma;

        public ViewPlane()
        {

        }
        public void set_hres(int h)
        {
            hres = h;
        }
        public void set_vres(int v)
        {
            vres = v;
        }
        public void set_pixel_size(float size)
        {
            s = size;
        }
        public void set_gamma(float g)
        {
            gamma = g;
            inv_gamma = 1 / g;
        }
    }
}