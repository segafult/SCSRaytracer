//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;
using System.Numerics;

namespace RayTracer
{
    class ThinLensCamera : Camera
    {
        Sampler depth_sampler;

        float d;
        float f; //Distance to focal plane
        float radius; //Radius of the lens
        float FOVERD;

        public ThinLensCamera () : base()
        {
            d = 250;
            f = 500;
            radius = 20;
            setUp(new Vect3D(0, 1, 0));
            FOVERD = f / d;
        }
        public void set_sampler(Sampler sp)
        {
            depth_sampler = sp;
            depth_sampler.map_samples_to_disk();
        }
        public void setVdp(float distance)
        {
            d = distance;
            FOVERD = f / d;
        }
        public void setRadius(float r)
        {
            radius = r;
        }
        public void setFocalLength(float f_arg)
        {
            f = f_arg;
            FOVERD = f / d;
        }
        public override void render_scene(World w)
        {
            RGBColor L = new RGBColor();
            Ray ray = new Ray();
            ViewPlane vp = w.vp;
            int depth = 0;

            Point2D sp = new Point2D();
            Point2D pp = new Point2D();
            Point2D dp = new Point2D();
            Point2D lp = new Point2D();

            //w.open_window(vp.hres, vp.vres);
            vp.s /= zoom;

            for(int r = 0; r < vp.vres-1; r++)
            {
                for(int c = 0; c < vp.hres-1; c++)
                {
                    L = GlobalVars.color_black;

                    for(int n = 0; n < vp.numSamples; n++)
                    {
                        //Sample on unit square
                        sp = vp.vpSampler.sample_unit_square();
                        //Sample in screenspace
                        pp.coords.X = vp.s * (c - vp.hres * 0.5f + sp.coords.X);
                        pp.coords.Y = vp.s * (r - vp.vres * 0.5f + sp.coords.Y);

                        dp = depth_sampler.sample_disk();
                        lp.coords.X = dp.coords.X * radius;
                        lp.coords.Y = dp.coords.Y * radius;

                        ray.origin = eye + lp.coords.X * u + lp.coords.Y * v;
                        ray.direction = ray_direction(pp, lp);
                        L += w.tracer.trace_ray(ray, depth);
                    }
                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel(r, c, L);
                    w.poll_events();
                }
            }
        }
        protected override void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo)
        {
            RGBColor L = new RGBColor();
            Ray ray = new Ray();
            ViewPlane vp = w.vp;
            int depth = 0;

            Point2D sp = new Point2D();
            Point2D pp = new Point2D();
            Point2D dp = new Point2D();
            Point2D lp = new Point2D();

            Sampler screen_sampler_clone = GlobalVars.vp_sampler.clone();
            Sampler lens_sampler_clone = depth_sampler.clone();

            //vp.s /= zoom;
            //Vector2 tmp2 = new Vector2(vp.hres * 0.5f, vp.vres*0.5f);
            for (int r = y1; r < y2; r++)
            {
                for (int c = x1; c < x2; c++)
                {
                    L = GlobalVars.color_black;

                    for (int n = 0; n < vp.numSamples; n++)
                    {
                        //Sample on unit square
                        sp = screen_sampler_clone.sample_unit_square();
                        //Sample in screenspace
                        //Vector2 tmp1 = new Vector2(c, r);
                        //pp.coords = vp.s * (tmp1 - tmp2 * sp.coords);
                        pp.coords.X = vp.s * (c - vp.hres * 0.5f + sp.coords.X);
                        pp.coords.Y = vp.s * (r - vp.vres * 0.5f + sp.coords.Y);

                        dp = lens_sampler_clone.sample_disk();
                        lp.coords.X = dp.coords.X * radius;
                        lp.coords.Y = dp.coords.Y * radius;
                        //lp.coords = dp.coords * radius;

                        ray.origin = eye + lp.coords.X * u + lp.coords.Y * v;
                        ray.direction = ray_direction(pp, lp);
                        L += w.tracer.trace_ray(ray, depth);
                    }
                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel(r, c, L);
                }
            }
        }

        private Vect3D ray_direction(Point2D pixel, Point2D lens)
        {
            Point2D p = new Point2D(pixel.coords.X*(f/d),pixel.coords.Y*(f/d));
            Vect3D dir = (p.coords.X - lens.coords.X) * u + (p.coords.Y - lens.coords.Y) * v - f * w;
            //Vector2 res = pixel.coords * FOVERD - lens.coords;
            //Vect3D dir = res.X * u + res.Y * v - f * w;
            dir.normalize();
            return dir;
        }

        public static ThinLensCamera LoadThinLensCamera(XmlElement camRoot)
        {
            ThinLensCamera toReturn = new ThinLensCamera();

            //Set up sampler according to what's provided
            string str_sampler = camRoot.GetAttribute("sampler");
            if (!str_sampler.Equals(""))
                toReturn.set_sampler(Sampler.LoadSampler(str_sampler));
            else
                toReturn.set_sampler(GlobalVars.vp_sampler);

            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                float vdp = (float)Convert.ToSingle(str_vdp);
                toReturn.setVdp(vdp);
            }
            XmlNode node_f = camRoot.SelectSingleNode("f");
            if(node_f != null)
            {
                string str_f = ((XmlText)node_f.FirstChild).Data;
                float f = (float)Convert.ToSingle(str_f);
                toReturn.setFocalLength(f);
            }
            XmlNode node_r = camRoot.SelectSingleNode("r");
            if(node_r != null)
            {
                string str_r = ((XmlText)node_r.FirstChild).Data;
                float r = (float)Convert.ToSingle(str_r);
                toReturn.setRadius(r);
            }
            return toReturn;
        }
    }
}
