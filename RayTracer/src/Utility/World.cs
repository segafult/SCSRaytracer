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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Class containing references to all world items
    /// </summary>
    public class World
    {
        public ViewPlane vp;
        public RGBColor bg_color;
        public Tracer tracer;

        public List<RenderableObject> renderList;
        public List<Light> lightList;
        public AmbientLight ambientLight;
        public Camera camera;

        public Bitmap drawPlan;
        public List<Bitmap> threadedBitmapList; //Only initialized when multithread rendering

        public World ()
        {
            vp = new ViewPlane();
            renderList = new List<RenderableObject>();
            lightList = new List<Light>();
            ambientLight = new AmbientLight();   
        }

        //Getters, setters, and adders


        /// <summary>
        /// Appends given render object to the end of the scene list
        /// </summary>
        /// <param name="o">Object to add</param>
        public void add_Object(RenderableObject o)
        {
            renderList.Add(o);
        }

        public void add_Light(Light l)
        {
            lightList.Add(l);
        }

        public void set_ambient_light(AmbientLight l)
        {
            ambientLight = l;
        }

        public void set_camera(Camera c)
        {
            camera = c;
        }

        public ShadeRec hit_barebones_objects(Ray ray)
        {
            ShadeRec sr = new ShadeRec(this);
            double t = GlobalVars.kHugeValue-1;
            double tmin = GlobalVars.kHugeValue;
            int num_objects = renderList.Count;

            for(int i = 0; i < num_objects; i++)
            {
                if(renderList[i].hit(ray,ref t, ref sr) && (t<tmin))
                {
                    sr.hit_an_object = true;
                    tmin = t;
                    sr.color = renderList[i].color;
                }
            }

            return sr;
        }

        /// <summary>
        /// Step 1 in rendering pipeline.
        /// Generic intersection detection for objectList called by a Tracer, returns ShadeRec describing intersection.
        /// </summary>
        /// <param name="ray">Ray for intersection calculation</param>
        /// <returns>ShadeRec object with all relevant information about object that was intersected.</returns>
        public ShadeRec hit_objects(Ray ray)
        {
            ShadeRec sr = new ShadeRec(this);
            double t = GlobalVars.kHugeValue-1.0;
            Normal normal = new Normal();
            Point3D local_hit_point = new Point3D();
            double tmin = GlobalVars.kHugeValue;
            int num_objects = renderList.Count;

            //Find the closest intersection point along the given ray
            for(int i = 0; i<num_objects; i++)
            {
                if (renderList[i].hit(ray, ref t, ref sr) && (t < tmin))
                {
                    sr.hit_an_object = true;
                    tmin = t;
                    sr.obj_material = renderList[i].getMaterial();
                    sr.hit_point = ray.origin + t * ray.direction;
                    normal = sr.normal;
                    local_hit_point = sr.hit_point_local;
                }
            }

            //If we hit an object, we need to store information about that object in sr'
            if(sr.hit_an_object)
            {
                sr.t = tmin;
                sr.normal = normal;
                sr.hit_point_local = local_hit_point;
            }

            return (sr);
        }
        /// <summary>
        /// Builds the world
        /// </summary>
        public void build()
        {
            vp.set_hres(1920);
            vp.set_vres(1080);
            vp.set_pixel_size(1.0F);
            vp.set_gamma(1.0F);
            vp.set_samples(16);
            vp.set_max_depth(5);
            RegularSampler mySampler = new RegularSampler(vp.numSamples);
            mySampler.generate_samples();
            vp.set_sampler(mySampler);

            bg_color = GlobalVars.color_black;
            tracer = new Whitted(this);

            PinholeCamera pinhole_ptr = new PinholeCamera();
            pinhole_ptr.setEye(new Point3D(0, 200, 500));
            pinhole_ptr.setLookat(new Point3D(0, 0, 0));
            pinhole_ptr.setVdp(850.0F);
            pinhole_ptr.setZoom(2.0F);
            pinhole_ptr.compute_uvw();
            set_camera(pinhole_ptr);

            PointLight light_ptr = new PointLight(new Point3D(0,150,259.8));
            light_ptr.setIntensity(5.0);
            light_ptr.setShadow(true);
            light_ptr.setColor(new RGBColor(1.0, 0.0, 0.0));
            add_Light(light_ptr);

            light_ptr = new PointLight(new Point3D(259.8, 150, -150));
            light_ptr.setIntensity(5.0);
            light_ptr.setShadow(true);
            light_ptr.setColor(new RGBColor(0.0, 1.0, 0.0));
            add_Light(light_ptr);

            light_ptr = new PointLight(new Point3D(-259.8, 150, -150));
            light_ptr.setIntensity(5.0);
            light_ptr.setShadow(true);
            light_ptr.setColor(new RGBColor(0.0, 0.0, 1.0));
            add_Light(light_ptr);

            ReflectiveShader phong_ptr = new ReflectiveShader();
            phong_ptr.setKa(0.5);
            phong_ptr.setKd(0.5);
            phong_ptr.setCd(new RGBColor(0.5, 0.5, 0.75));
            phong_ptr.setExp(20.0);
            phong_ptr.setKs(0.75);
            phong_ptr.setReflectivity(0.75);
            phong_ptr.setCr(new RGBColor(0.5, 0.5, 0.75));
            Sphere sphere_ptr = new Sphere(new Point3D(0, 5, 0), 40);
            sphere_ptr.setMaterial(phong_ptr);
            add_Object(sphere_ptr);
            
            /*phong_ptr = new PhongShader();
            phong_ptr.setKa(0.25f);
            phong_ptr.setKd(0.65f);
            phong_ptr.setCd(new RGBColor(1, 0, 0));
            phong_ptr.setExp(20.0f);
            phong_ptr.setKs(0.2f);*/
            sphere_ptr = new Sphere(new Point3D(-150, 30, 15), 60);
            sphere_ptr.setMaterial(phong_ptr);
            add_Object(sphere_ptr);

            /*phong_ptr = new PhongShader();
            phong_ptr.setKa(0.25f);
            phong_ptr.setKd(0.65f);
            phong_ptr.setExp(20.0f);
            phong_ptr.setKs(0.2f);
            phong_ptr.setCd(new RGBColor(0, 1, 0));*/
            sphere_ptr = new Sphere(new Point3D(150, 5, -10), 50);
            sphere_ptr.setMaterial(phong_ptr);
            add_Object(sphere_ptr);

            MatteShader matte_ptr = new MatteShader();
            matte_ptr.setKa(0.25F);
            matte_ptr.setKd(0.65F);
            matte_ptr.setCd(new RGBColor(1.0, 1.0, 1.0));
            Plane plane_ptr = new Plane(new Point3D(10,-32 ,0),new Normal(0,1,0));
            plane_ptr.setMaterial(phong_ptr);
            add_Object(plane_ptr);
        }

        /// <summary>
        /// Sets up rendering field, be it screen or image
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="vres"></param>
        public void open_window(int hres, int vres)
        {
            drawPlan = new Bitmap(hres, vres);
        }

        /// <summary>
        /// Sets up a rendering field for a multithreaded render
        /// </summary>
        /// <param name="hres">Horizontal resolution</param>
        /// <param name="vres">Vertical resolution</param>
        /// <param name="numThreads">Number of threads (2, 4, or 16)</param>
        public void open_window_threaded(int hres, int vres, int numThreads)
        {
            drawPlan = new Bitmap(hres, vres);
            threadedBitmapList = new List<Bitmap>();
            int xsize = 0;
            int ysize = 0;

            //If there are 2 threads, divide the screen into halves.
            if(numThreads == 2)
            {
                xsize = hres / 2;
                ysize = vres;
            }
            //If there are 4 threads, divide the screen into quadrants
            else if(numThreads == 4)
            {
                xsize = hres / 2;
                ysize = vres / 2;
            }
            else if(numThreads == 8)
            {
                xsize = hres / 4;
                ysize = vres / 2;
            }
            else if(numThreads == 16)
            {
                xsize = hres / 4;
                ysize = vres / 4;
            }

            for (int i = 0; i < numThreads; i++)
            {
                threadedBitmapList.Add(new Bitmap(xsize, ysize));
            }
        }

        /// <summary>
        /// Performs the required colorspace conversions
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="pixel_color"></param>
        public void display_pixel(int row, int column, RGBColor pixel_color)
        {
            RGBColor disp_color = pixel_color.clamp();
            drawPlan.SetPixel(column, row, Color.FromArgb(255, (int)(disp_color.r * 250), (int)(disp_color.g * 250), (int)(disp_color.b * 250)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void display_pixel_threadsafe(int row, int column, RGBColor pixel_color, ref BitmapData bmp)
        {
            RGBColor disp_color = pixel_color.clamp();
            byte r = (byte)(255 * disp_color.r);
            byte g = (byte)(255 * disp_color.g);
            byte b = (byte)(255 * disp_color.b);
            int stride = bmp.Stride;
            unsafe
            {
                byte* ptr = (byte*)bmp.Scan0;
                ptr[(column * 3) + row * stride] = b;
                ptr[(column * 3) + row * stride + 1] = g;
                ptr[(column * 3) + row * stride + 2] = r;
            }
            //bmp.SetPixel(column, row, Color.FromArgb(255, (int)(disp_color.r * 250), (int)(disp_color.g * 250), (int)(disp_color.b * 250)));
        }

        public void join_bitmaps(int numThreads)
        {
            Graphics joinedImage = Graphics.FromImage(drawPlan);
            if(numThreads == 2)
            {
                joinedImage.Clear(Color.Black);
                joinedImage.DrawImageUnscaled(threadedBitmapList[0], new Point(0, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[1], new Point(vp.hres / 2, 0));
            }
            else if(numThreads == 4)
            {
                joinedImage.Clear(Color.Black);
                joinedImage.DrawImageUnscaled(threadedBitmapList[0], new Point(0, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[1], new Point(vp.hres / 2, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[2], new Point(0, vp.vres / 2));
                joinedImage.DrawImageUnscaled(threadedBitmapList[3], new Point(vp.hres / 2, vp.vres / 2));
            }
            else if(numThreads == 8)
            {
                joinedImage.Clear(Color.Black);
                joinedImage.DrawImageUnscaled(threadedBitmapList[0], new Point(0, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[1], new Point(vp.hres/4, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[2], new Point(vp.hres/2, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[3], new Point(3*vp.hres/4, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[4], new Point(0, vp.vres/2));
                joinedImage.DrawImageUnscaled(threadedBitmapList[5], new Point(vp.hres/4, vp.vres/2));
                joinedImage.DrawImageUnscaled(threadedBitmapList[6], new Point(vp.hres/2, vp.vres/2));
                joinedImage.DrawImageUnscaled(threadedBitmapList[7], new Point(3*vp.hres/4, vp.vres/2));
            }

        }
    } 
}
