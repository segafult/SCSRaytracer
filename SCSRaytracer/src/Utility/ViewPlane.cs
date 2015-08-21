//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    sealed class ViewPlane
    {
        public int hres;
        public int vres;
        public float s;
        public float gamma;
        public float inv_gamma;
        public int maxDepth;

        public int numSamples;
        public Sampler vpSampler;

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
        public void set_numSamples(int samples)
        {
            numSamples = samples;
        }
        public void set_sampler(Sampler smp)
        {
            vpSampler = smp;
        }
        public void set_samples(int samples)
        {
            numSamples = samples;
        }
        public void set_max_depth(int mdepth)
        {
            maxDepth = mdepth;
        }
    }
}