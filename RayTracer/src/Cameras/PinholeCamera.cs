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
using System.Threading;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class PinholeCamera : Camera
    {
        private double d; //Distance between pinhole and viewplane
        private double zoom; //Zoom factor

        public void setVdp(double distance)
        {
            d = distance;
        }
        public void setZoom(double z)
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

        public override void render_scene_multithreaded(World w, int numThreads)
        {
            ViewPlane vp = w.vp;
            w.open_window_threaded(vp.hres, vp.vres, numThreads);

            vp.s /= zoom;
            List<Thread> threads = new List<Thread>();

            if (numThreads == 4)
            {
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (int)vp.hres / 2, 0, (int)vp.vres / 2, 0)));
                threads.Add(new Thread(() => render_scene_fragment(w, (int)vp.hres / 2, vp.hres, 0, (int)vp.vres / 2, 1)));
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (int)vp.hres / 2, (int)vp.vres / 2, vp.vres, 2)));
                threads.Add(new Thread(() => render_scene_fragment(w, (int)vp.hres / 2, vp.hres, (int)vp.vres / 2, vp.vres, 3)));
                foreach (Thread t in threads)
                {
                    t.Start();
                }
            }
            else
            {
                Console.WriteLine("Multithreading only supported for 4 threads");
            }

            //Spinwait for all threads
            foreach (Thread t in threads)
            {
                t.Join();
            }

            w.join_bitmaps(numThreads);
        }

        /// <summary>
        /// Subroutine for rendering a given rectangular fragment of a scene, called when multithreading.
        /// </summary>
        /// <param name="w">World to render</param>
        /// <param name="x1">Starting x coordinate (minimum 0)</param>
        /// <param name="x2">Ending x coordinate</param>
        /// <param name="y1">Starting y coordinate (minimum 0)</param>
        /// <param name="y2">Ending y coordinate</param>
        /// <param name="threadNo">Thread number subroutine has been called on. Used for storing subimages in bitmap list</param>
        protected override void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo)
        {
            Bitmap renderBmp = new Bitmap(x2 - x1, y2 - y1);
            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye, new Vect3D(0, 0, 0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space
            exposure_time = 1.0;

            for (int row = y1; row < y2; row++)
            {
                for (int column = x1; column < x2; column++)
                {
                    L = GlobalVars.color_black; //Start with no color, everything is additive

                    for (int sample = 0; sample < vp.numSamples; sample++)
                    {
                        sp = w.vp.vpSampler.sample_unit_square();
                        pp.x = w.vp.s * (column - 0.5 * vp.hres + sp.x);
                        pp.y = w.vp.s * (row - 0.5 * vp.vres + sp.y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel_threadsafe(row, column, L, ref renderBmp);
                }
            }
            w.threadedBitmapList[threadNo] = renderBmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vect3D ray_direction(Point2D p)
        {
            Vect3D dir = p.x * u + p.y * v - d * w;
            dir.normalize();
            return dir;
        }
    }
}
