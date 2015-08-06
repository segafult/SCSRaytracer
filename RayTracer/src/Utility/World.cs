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

        public List<RenderableObject> renderList;  //Renderlist is the actual list of objects to render
        public List<RenderableObject> objectList;  //Objectlist isn't rendered, and instead serves to hold objects to be instanced

        public List<Light> lightList;
        public AmbientLight ambientLight;
        public Camera camera;

        public List<Material> materialList;

        public Bitmap drawPlan;
        public List<Bitmap> threadedBitmapList; //Only initialized when multithread rendering

        public World ()
        {
            vp = new ViewPlane();
            renderList = new List<RenderableObject>();
            objectList = new List<RenderableObject>();

            lightList = new List<Light>();
            materialList = new List<Material>();
            ambientLight = new AmbientLight();
        }

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
            /*
            vp.set_hres(1920);
            vp.set_vres(1080);
            vp.set_pixel_size(1.0F);
            vp.set_gamma(1.0F);
            vp.set_samples(16);
            vp.set_max_depth(5);
            RegularSampler mySampler = new RegularSampler(vp.numSamples);
            mySampler.generate_samples();
            vp.set_sampler(mySampler);
            

            
            tracer = new Whitted(this);

            PinholeCamera pinhole_ptr = new PinholeCamera();
            pinhole_ptr.setEye(new Point3D(100, 200, 500));
            pinhole_ptr.setLookat(new Point3D(0, 0, 0));
            pinhole_ptr.setVdp(850.0);
            pinhole_ptr.setZoom(2.5);
            pinhole_ptr.compute_uvw();
            set_camera(pinhole_ptr);
            */
            bg_color = GlobalVars.color_black;

            /*
            PointLight light_ptr = new PointLight(new Point3D(0,500,259.8));
            light_ptr.setIntensity(5.0);
            light_ptr.setShadow(true);
            light_ptr.setColor(new RGBColor(1.0, 1.0, 1.0));
            add_Light(light_ptr);
            */

            XMLProcessor sceneLoader = new XMLProcessor(GlobalVars.inFile, this);
            sceneLoader.LoadMaterials();
            if (GlobalVars.verbose)
            {
                Console.WriteLine("Materials loaded:");
                foreach (Material m in materialList)
                {
                    Console.WriteLine(m.ToString());
                }
            }

            sceneLoader.LoadObjects();
            if(GlobalVars.verbose)
            {
                Console.WriteLine("Object definitions loaded:");
                foreach(RenderableObject o in objectList)
                {
                    Console.WriteLine(o.ToString());
                }
            }

            sceneLoader.LoadWorld();
            if(GlobalVars.verbose)
            {
                Console.WriteLine("Scene loaded!");
                foreach(RenderableObject o in renderList)
                {
                    Console.WriteLine(o.ToString());
                }
            }
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

        public Material getMaterialById(string idarg)
        {
            //idarg will be null if no node is returned by the XmlProcessor
            if (idarg == null)
            {
                return new MatteShader();
            }
            else
            {
                int numMats = materialList.Count;
                bool foundMat = false;

                int matIndex = 0;
                for (int i = 0; i < numMats; i++)
                {
                    if (materialList[i].id.Equals(idarg))
                    {
                        foundMat = true;
                        matIndex = i;
                        break;
                    }
                }

                if (foundMat)
                {
                    return materialList[matIndex];
                }
                else
                {
                    return new MatteShader();
                }
            }
        }
        public RenderableObject getObjectById(string objarg)
        {
            if(objarg == null)
            {
                return null;
            }
            else
            {
                int numObjs = objectList.Count;
                bool foundObj = false;
                int objIndex = 0;

                for(int i = 0; i < numObjs; i++)
                {
                    if (objectList[i].id.Equals(objarg))
                    {
                        foundObj = true;
                        objIndex = i;
                        break;
                    }
                }

                if(foundObj)
                {
                    return objectList[objIndex];
                }
                else
                {
                    return null;
                }
            }
        }
    } 
}
