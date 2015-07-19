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
        public double s;
        public double gamma;
        public double inv_gamma;
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
        public void set_pixel_size(double size)
        {
            s = size;
        }
        public void set_gamma(double g)
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