//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace RayTracer
{
    class PinholeCamera : Camera
    {
        private float d; //Distance between pinhole and viewplane

        public PinholeCamera() : base()
        {
            d = 850;
        }
        public void setVdp(float distance)
        {
            d = distance;
        }

        public override void render_scene(World w)
        {
            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye,new Vect3D(0,0,0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space

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
                        pp.coords.X = w.vp.s * (column - 0.5f * vp.hres + sp.coords.X);
                        pp.coords.Y = w.vp.s * (row - 0.5f * vp.vres + sp.coords.Y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel(row, column, L);

                    //Poll events in live render view
                    w.poll_events();
                }
            }
        }

        /// <summary>
        /// Subroutine for rendering a given rectangular fragment of a scene, called when multithreading.
        /// </summary>
        protected override void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo)
        {
            //For thread safety, use a local bitmap, and lock image for direct byte writing.
            //Bitmap renderBmp = new Bitmap(x2 - x1, y2 - y1);
            //BitmapData renderData = renderBmp.LockBits(new Rectangle(0,0,renderBmp.Width,renderBmp.Height), 
            //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //To avoid clashes with other threads accessing sampler, clone the main world sampler
            Sampler localSampler = w.vp.vpSampler.clone();

            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye, new Vect3D(0, 0, 0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space
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
                        pp.coords.X = w.vp.s * (column + x1 - 0.5f * vp.hres + sp.coords.X);
                        pp.coords.Y = w.vp.s * (row + y1 - 0.5f * vp.vres + sp.coords.Y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;

                    //w.display_pixel_threadsafe(row, column, L, ref renderData);
                    //w.update_liveimage_threadsafe(row+y1, column+x1, L);
                    w.display_pixel(row + y1, column + x1, L);
                }
            }
            //renderBmp.UnlockBits(renderData);
            //w.threadedBitmapList[threadNo] = renderBmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vect3D ray_direction(Point2D p)
        {
            Vect3D dir = p.coords.X * u + p.coords.Y * v - d * w;
            dir.normalize();
            return dir;
        }

        public static PinholeCamera LoadPinholeCamera(XmlElement camRoot)
        {
            PinholeCamera toReturn = new PinholeCamera();

            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                float vdp = (float)Convert.ToSingle(str_vdp);
                toReturn.setVdp(vdp);
            }
            return toReturn;
        }
    }
}
