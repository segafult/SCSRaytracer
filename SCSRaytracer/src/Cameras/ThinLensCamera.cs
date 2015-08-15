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
using System.Xml;

namespace RayTracer
{
    class ThinLensCamera : Camera
    {
        Sampler depth_sampler;
        double d;
        double f; //Distance to focal plane
        double radius; //Radius of the lens

        public ThinLensCamera () : base()
        {
            d = 250;
            f = 500;
            radius = 20;
            setUp(new Vect3D(0, 1, 0));
        }
        public void set_sampler(Sampler sp)
        {
            depth_sampler = sp;
            depth_sampler.map_samples_to_disk();
        }
        public void setVdp(double distance)
        {
            d = distance;
        }
        public void setRadius(double r)
        {
            radius = r;
        }
        public void setFocalLength(double f_arg)
        {
            f = f_arg;
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
                        pp.x = vp.s * (c - vp.hres * 0.5 + sp.x);
                        pp.y = vp.s * (r - vp.vres * 0.5 + sp.y);

                        dp = depth_sampler.sample_disk();
                        lp.x = dp.x * radius;
                        lp.y = dp.y * radius;

                        ray.origin = eye + lp.x * u + lp.y * v;
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
                        pp.x = vp.s * (c - vp.hres * 0.5 + sp.x);
                        pp.y = vp.s * (r - vp.vres * 0.5 + sp.y);

                        dp = lens_sampler_clone.sample_disk();
                        lp.x = dp.x * radius;
                        lp.y = dp.y * radius;

                        ray.origin = eye + lp.x * u + lp.y * v;
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
            Point2D p = new Point2D(pixel.x*(f/d),pixel.y*(f/d));
            Vect3D dir = (p.x - lens.x) * u + (p.y - lens.y) * v - f * w;
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
                double vdp = Convert.ToDouble(str_vdp);
                toReturn.setVdp(vdp);
            }
            XmlNode node_f = camRoot.SelectSingleNode("f");
            if(node_f != null)
            {
                string str_f = ((XmlText)node_f.FirstChild).Data;
                double f = Convert.ToDouble(str_f);
                toReturn.setFocalLength(f);
            }
            XmlNode node_r = camRoot.SelectSingleNode("r");
            if(node_r != null)
            {
                string str_r = ((XmlText)node_r.FirstChild).Data;
                double r = Convert.ToDouble(str_r);
                toReturn.setRadius(r);
            }
            return toReturn;
        }
    }
}
