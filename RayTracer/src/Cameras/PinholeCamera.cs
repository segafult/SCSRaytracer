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
    public class PinholeCamera : Camera
    {
        private float d; //Distance between pinhole and viewplane
        private float zoom; //Zoom factor

        public void setVdp(float distance)
        {
            d = distance;
        }
        public void setZoom(float z)
        {
            zoom = z;
        }

        public override void render_scene(World w)
        {
            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye,new Vect3D(0,0,0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space
            exposure_time = 1.0;

            w.open_window(vp.hres, vp.vres);
            vp.s /= zoom;

            for(int row = 0; row < vp.vres; row++)
            {
                for(int column = 0; column < vp.hres; column++)
                {
                    L = GlobalVars.color_black; //Start with no color, everything is additive

                    for(int sample = 0; sample < vp.numSamples; sample ++)
                    {
                        sp = w.vp.vpSampler.sample_unit_square();
                        pp.x = w.vp.s * (column - 0.5 * vp.hres + sp.x);
                        pp.y = w.vp.s * (row - 0.5 * vp.vres + sp.y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel(row, column, L);
                }
            }
        }
        private Vect3D ray_direction(Point2D p)
        {
            Vect3D dir = p.x * u + p.y * v - d * w;
            dir.normalize();
            return dir;
        }
    }
}
