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
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Xml;

namespace RayTracer
{
    public class PinholeCamera : Camera
    {
        private double d; //Distance between pinhole and viewplane
        private double zoom; //Zoom factor

        public PinholeCamera()
        {
            d = 850;
            zoom = 1.0;
            this.setEye(new Point3D(0, 0, 0));
            this.setExposure(1.0);
            this.setLookat(new Point3D(0, 0, 500));
            this.compute_uvw();
        }
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

            if(numThreads == 2)
            {
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (int)vp.hres / 2, 0, vp.vres, 0)));
                threads.Add(new Thread(() => render_scene_fragment(w, (int)vp.hres / 2, vp.hres, 0, vp.vres, 1)));
            }
            else if (numThreads == 4)
            {
                threads.Add(new Thread(() => render_scene_fragment(w, 0, vp.hres / 2, 0, vp.vres / 2, 0)));
                threads.Add(new Thread(() => render_scene_fragment(w, vp.hres / 2, vp.hres, 0, vp.vres / 2, 1)));
                threads.Add(new Thread(() => render_scene_fragment(w, 0, vp.hres / 2, vp.vres / 2, vp.vres, 2)));
                threads.Add(new Thread(() => render_scene_fragment(w, vp.hres / 2, vp.hres, vp.vres / 2, vp.vres, 3)));
            }
            else if (numThreads == 8)
            {
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (vp.hres / 4), 0, vp.vres / 2, 0)));
                threads.Add(new Thread(() => render_scene_fragment(w, (vp.hres / 4), (vp.hres / 2), 0, vp.vres / 2, 1)));
                threads.Add(new Thread(() => render_scene_fragment(w, (vp.hres / 2), (3*vp.hres / 4), 0, vp.vres / 2, 2)));
                threads.Add(new Thread(() => render_scene_fragment(w, (3 * vp.hres / 4), vp.hres, 0, vp.vres / 2, 3)));
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (vp.hres / 4), vp.vres/2, vp.vres, 4)));
                threads.Add(new Thread(() => render_scene_fragment(w, (vp.hres / 4), (vp.hres / 2), vp.vres / 2, vp.vres, 5)));
                threads.Add(new Thread(() => render_scene_fragment(w, (vp.hres / 2), (3 * vp.hres / 4), vp.vres / 2, vp.vres, 6)));
                threads.Add(new Thread(() => render_scene_fragment(w, (3 * vp.hres / 4), vp.hres, vp.vres / 2, vp.vres, 7)));
            }
            else
            {
                Console.WriteLine("Multithreading only supported for 2, 4, or 8 threads");
            }

            foreach (Thread t in threads)
            {
                t.Start();
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
        protected override void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo)
        {
            //For thread safety, use a local bitmap, and lock image for direct byte writing.
            Bitmap renderBmp = new Bitmap(x2 - x1, y2 - y1);
            BitmapData renderData = renderBmp.LockBits(new Rectangle(0,0,renderBmp.Width,renderBmp.Height), 
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //To avoid clashes with other threads accessing sampler, clone the main world sampler
            Sampler localSampler = w.vp.vpSampler.clone();

            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye, new Vect3D(0, 0, 0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space
            exposure_time = 1.0;
            int height = y2 - y1;
            int width = x2 - x1;

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    L = GlobalVars.color_black; //Start with no color, everything is additive

                    for (int sample = 0; sample < vp.numSamples; sample++)
                    {
                        sp = localSampler.sample_unit_square();
                        pp.x = w.vp.s * (column + x1 - 0.5 * vp.hres + sp.x);
                        pp.y = w.vp.s * (row + y1 - 0.5 * vp.vres + sp.y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;

                    w.display_pixel_threadsafe(row, column, L, ref renderData);
                }
            }
            renderBmp.UnlockBits(renderData);
            w.threadedBitmapList[threadNo] = renderBmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vect3D ray_direction(Point2D p)
        {
            Vect3D dir = p.x * u + p.y * v - d * w;
            dir.normalize();
            return dir;
        }

        public static PinholeCamera LoadPinholeCamera(XmlElement camRoot)
        {
            PinholeCamera toReturn = new PinholeCamera();

            XmlNode node_zoom = camRoot.SelectSingleNode("zoom");
            if (node_zoom != null)
            {
                string str_zoom = ((XmlText)node_zoom.FirstChild).Data;
                double zoom = Convert.ToDouble(str_zoom);
                toReturn.setZoom(zoom);
            }
            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                double vdp = Convert.ToDouble(str_vdp);
                toReturn.setVdp(vdp);
            }
            return toReturn;
        }
    }
}
