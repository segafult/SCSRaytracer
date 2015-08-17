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
using System.Numerics;
using System.Threading;


namespace RayTracer
{
    /// <summary>
    /// Class containing references to all world items
    /// </summary>
    class World
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

        LiveViewer live_view;
        
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
            float t = GlobalVars.kHugeValue-1.0f;
            Normal normal = new Normal();
            Point3D local_hit_point = new Point3D();
            float tmin = GlobalVars.kHugeValue;
            int num_objects = renderList.Count;
            Material closestmat = null;

            //Find the closest intersection point along the given ray
            for(int i = 0; i<num_objects; i++)
            {
                if (renderList[i].hit(ray, ref t, ref sr) && (t < tmin))
                {
                    sr.hit_an_object = true;
                    tmin = t;
                    closestmat = sr.obj_material;
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
                sr.obj_material = closestmat;
            }

            return (sr);
        }
        /// <summary>
        /// Builds the world
        /// </summary>
        public void build()
        {
            bg_color = GlobalVars.color_black;

            if (GlobalVars.inFile != null)
            {

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
                if (GlobalVars.verbose)
                {
                    Console.WriteLine("Object definitions loaded:");
                    foreach (RenderableObject o in objectList)
                    {
                        Console.WriteLine(o.ToString());
                    }
                }

                sceneLoader.LoadWorld();
                if (GlobalVars.verbose)
                {
                    Console.WriteLine("Scene loaded!");
                    foreach (RenderableObject o in renderList)
                    {
                        Console.WriteLine(o.ToString());
                    }
                }

                //Sphere mysphere = new Sphere(new Point3D(0, 0, 0), 1000);
                //mysphere.setMaterial(getMaterialById("myreflective"));
                //add_Object(mysphere);
                //((ThinLensCamera)camera).set_sampler(vp.vpSampler);

                UniformGrid mygrid = new UniformGrid();

                Plane groundplane = new Plane(new Point3D(0, -100, 0), new Normal(0, 1, 0));
                groundplane.setMaterial(new DebugCheckerboard());
                add_Object(groundplane);

                SphericalMapper sphere_map = new SphericalMapper();
                Image my_image = new Image();
                my_image.loadFromFile("E:\\earth.jpg");

                ImageTexture my_tex = new ImageTexture();
                my_tex.setMapper(sphere_map);
                my_tex.set_image(my_image);

                MatteShader mymatte = new MatteShader();
                mymatte.setCd(my_tex);
                mymatte.setKa(0.9f);

                Sphere mysphere = new Sphere(new Point3D(0, 0, 0), 20);
                mysphere.setMaterial(mymatte);

                Instance myinstance = new Instance(mysphere);
                myinstance.scale(new Vect3D(5, 5, 5));
                myinstance.rotate(new Vect3D(23.5f, 90, 0));
                mygrid.add_object(myinstance);
                //add_Object(myinstance);

                Image my_skybox = new Image();
                my_skybox.loadFromFile("E:\\skybox.jpg");

                my_tex = new ImageTexture();
                my_tex.setMapper(sphere_map);
                my_tex.set_image(my_skybox);

                MatteShader skymatte = new MatteShader();
                skymatte.setCd(my_tex);
                skymatte.setKa(1.0f);

                Sphere skybox = new Sphere(new Point3D(0, 0, 0),1000);
                skybox.setMaterial(skymatte);
                mygrid.add_object(skybox);
                //add_Object(skybox);

                mygrid.setup_cells();
                add_Object(mygrid);

                
            }
            //Custom build function if no input file specified


            
            else
            {
                    ///---------------------------------------------------------------------------------------
                    ///Insert your build function here





                    ///----------------------------------------------------------------------------------------
            }
        }

        /// <summary>
        /// Sets up rendering field, be it screen or image
        /// </summary>
        /// <param name="hres"></param>
        /// <param name="vres"></param>
        public void open_window(int hres, int vres)
        {
            //drawPlan = new Bitmap(hres, vres);
            live_view = new LiveViewer(this);
            live_view.set_up_liveview();
        }

        /*
        /// <summary>
        /// Sets up a rendering field for a multithreaded render
        /// </summary>
        /// <param name="hres">Horizontal resolution</param>
        /// <param name="vres">Vertical resolution</param>
        /// <param name="numThreads">Number of threads (2, 4, or 16)</param>
        public void open_window_threaded(int hres, int vres, int numThreads)
        {
            //drawPlan = new Bitmap(hres, vres);
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

            //Set up the live view window
            live_view = new LiveViewer(this);
            live_view.set_up_liveview();
        }
        */

        /// <summary>
        /// Performs the required colorspace conversions
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="pixel_color"></param>
        public void display_pixel(int row, int column, RGBColor pixel_color)
        {
            RGBColor disp_color = new RGBColor(pixel_color.vals * 255.0f);
            Vector3 cols = disp_color.clamp().vals;
            byte r = (byte)cols.X;
            byte g = (byte)cols.Y;
            byte b = (byte)cols.Z;
            int x = column;
            int y = vp.vres-1-row;
            live_view.live_image.SetPixel((uint)column, (uint)(vp.vres-1-row), new SFML.Graphics.Color(r, g, b));
        }

        public Thread get_window_thread()
        {
            return live_view.get_thread();
        }
        public void poll_events()
        {
            live_view.poll_events();
        }
        public void save_displayed_image(string path)
        {
            live_view.live_image.SaveToFile(path);
        }


        /*
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
        
        public void update_liveimage_threadsafe(int row, int column, RGBColor pixel_color)
        {
            RGBColor disp_color = pixel_color.clamp();
            
            byte[] color = new byte[4];
            byte r = (byte)(255 * disp_color.r);
            byte g = (byte)(255 * disp_color.g);
            byte b = (byte)(255 * disp_color.b);
            //live_image.SetPixel((uint)column, (uint)(vp.vres-row-1), new SFML.Graphics.Color(r, g, b));
            
        }
        
        public void join_bitmaps(int numThreads)
        {
            Graphics joinedImage = Graphics.FromImage(drawPlan);
            if(numThreads == 2)
            {
                joinedImage.Clear(System.Drawing.Color.Black);
                joinedImage.DrawImageUnscaled(threadedBitmapList[0], new Point(0, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[1], new Point(vp.hres / 2, 0));
            }
            else if(numThreads == 4)
            {
                joinedImage.Clear(System.Drawing.Color.Black);
                joinedImage.DrawImageUnscaled(threadedBitmapList[0], new Point(0, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[1], new Point(vp.hres / 2, 0));
                joinedImage.DrawImageUnscaled(threadedBitmapList[2], new Point(0, vp.vres / 2));
                joinedImage.DrawImageUnscaled(threadedBitmapList[3], new Point(vp.hres / 2, vp.vres / 2));
            }
            else if(numThreads == 8)
            {
                joinedImage.Clear(System.Drawing.Color.Black);
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
        */

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
