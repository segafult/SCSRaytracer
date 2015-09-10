//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;

namespace SCSRaytracer
{
    abstract class Camera
    {
        protected Point3D eye;
        protected Point3D lookat;  
        protected Vect3D up;
        protected Vect3D u, v, w;
        protected float exposure_time;
        protected float zoom;
        protected ConcurrentQueue<RenderFragmentParameters> taskQueue;

        public Camera()
        {
            this.setUp(new Vect3D(0, 1, 0));
            this.setEye(new Point3D(0, 0, 0));
            this.setExposure(1.0f);
            this.setLookat(new Point3D(0, 0, 500));
            this.setExposure(1.0f);
            this.setZoom(1.0f);
            this.compute_uvw();
        }
        //Getters and setters
        public void setEye(Point3D e) { eye = new Point3D(e); }
        public void setLookat(Point3D l) { lookat = new Point3D(l); }
        public void setUp(Vect3D u) { up = new Vect3D(u); }
        public void setExposure(float e) { exposure_time = e; }
        public void setZoom(float z) { zoom = z; }

        public void compute_uvw() 
        {
            up = new Vect3D(0, 1, 0);
            w = eye - lookat;
            w.normalize();
            u = up ^ w;
            u.normalize();
            v = w ^ u;
        }

        public abstract void render_scene(World w);
        public virtual void render_scene_multithreaded(World w, int numThreads)
        {
            ViewPlane vp = w.vp;

            if(GlobalVars.frameno == 0)
                vp.s /= zoom;

            List<RenderFragmentParameters> threads = new List<RenderFragmentParameters>();
            taskQueue = new ConcurrentQueue<RenderFragmentParameters>();

            //Find out how many chunks the screen can be divided into;
            int hchunks = (int)Math.Floor((decimal)vp.hres / (decimal)GlobalVars.fragment_size);
            int vchunks = (int)Math.Floor((decimal)vp.vres / (decimal)GlobalVars.fragment_size);
            //Find out the size of the boundaries which do not fit into chunks
            int hrem = vp.hres % GlobalVars.fragment_size;
            int vrem = vp.vres % GlobalVars.fragment_size;

            int threadNo = 0; //Label for each thread created

            //Queue up render chunks
            int h0, h1, v0, v1;
            RenderFragmentParameters parameters;
            for(int v = 0; v < vchunks; v++)
            {
                for(int h = 0; h < hchunks; h++)
                {
                    //Queue up threads
                    h0 = GlobalVars.fragment_size * h;
                    h1 = GlobalVars.fragment_size * (h + 1);
                    v0 = GlobalVars.fragment_size * v;
                    v1 = GlobalVars.fragment_size * (v + 1);
                    parameters = new RenderFragmentParameters(w, h0, h1, v0, v1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                    threadNo++;
                }
                //Add in additional fragments for the right screen edge if resolution isn't nicely
                //divisible
                if(hrem != 0)
                {
                    h0 = GlobalVars.fragment_size * hchunks;
                    h1 = vp.hres;
                    v0 = GlobalVars.fragment_size * v;
                    v1 = GlobalVars.fragment_size * (v + 1);
                    parameters = new RenderFragmentParameters(w, h0, h1, v0, v1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
            }
            //Add in additional fragments for the top edge if resolution isn't nicely divisible
            if(vrem != 0)
            {
                for(int h = 0; h < hchunks; h++)
                {
                    h0 = GlobalVars.fragment_size * h;
                    h1 = GlobalVars.fragment_size * (h + 1);
                    v0 = GlobalVars.fragment_size * vchunks;
                    v1 = vp.vres;
                    parameters = new RenderFragmentParameters(w, h0, h1, v0, v1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
                //Add in corner edge fragment if the right edge of the screen wasn't nicely divisible
                if (hrem != 0)
                {
                    h0 = GlobalVars.fragment_size * hchunks;
                    h1 = vp.hres;
                    v0 = GlobalVars.fragment_size * vchunks;
                    v1 = vp.vres;
                    parameters = new RenderFragmentParameters(w, h0, h1, v0, v1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
            }

            int num_to_dequeue = (numThreads < threadNo) ? numThreads : threadNo;

            //Temporary list to prevent race conditions
            RenderFragmentParameters[] initialThreads = new RenderFragmentParameters[num_to_dequeue];

            for(int i = 0; i < num_to_dequeue; i++)
            {
                while(!taskQueue.TryDequeue(out initialThreads[i])) { // ensure every dequeue succeeds
                }
            }

            foreach(RenderFragmentParameters r in initialThreads) //Away we go
            {
                r.Begin();
            }

            //Set the thread priority for main thread to low
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            //Spinwait for all threads
            bool allDone = false;
            do
            {
                allDone = true;
                foreach (RenderFragmentParameters t in threads)
                {
                    if (t.t.IsAlive)
                    {
                        allDone = false;
                    }
                }
                w.poll_events();

            } while (!allDone);

            w.live_view.live_image = new SFML.Graphics.Image((uint)vp.hres, (uint)vp.vres, w.image);
            
        }
        public abstract void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo);
        public void dequeue_next_render_fragment()
        {
            RenderFragmentParameters r;
            while(!taskQueue.TryDequeue(out r)) //Keep trying to dequeue if concurrent queue is locked
            {
                //Check that the queue has something in it. If it's empty then no need to dequeue next render task
                if (taskQueue.IsEmpty) break;
            }
            if (r != null) r.Begin();
        }

        public static Camera LoadCamera(XmlElement camRoot)
        {
            string cam_type = camRoot.GetAttribute("type");
            Camera toReturn = new PinholeCamera();
            //If no camera type is defined, then return a default pinhole camera.
            if (!cam_type.Equals(""))
            {
                //Load subtype specific parameters in their own methods
                if (cam_type.Equals("pinhole"))
                {
                    toReturn = PinholeCamera.LoadPinholeCamera(camRoot);
                }
                else if(cam_type.Equals("thinlens"))
                {
                    toReturn = ThinLensCamera.LoadThinLensCamera(camRoot);
                }
                else
                {
                    Console.WriteLine("Unknown camera type: " + cam_type);
                }

                XmlNode node_zoom = camRoot.SelectSingleNode("zoom");
                if (node_zoom != null)
                {
                    string str_zoom = ((XmlText)node_zoom.FirstChild).Data;
                    float zoom = (float)Convert.ToSingle(str_zoom);
                    toReturn.setZoom(zoom);
                }

                //Load common attributes afterwards.
                XmlNode node_point = camRoot.SelectSingleNode("point");
                if (node_point != null)
                {
                    string str_point = ((XmlText)node_point.FirstChild).Data;
                    Point3D point = Point3D.FromCsv(str_point);
                    //if (point != null)
                    //{
                        toReturn.setEye(point);
                    //}
                }
                XmlNode node_lookat = camRoot.SelectSingleNode("lookat");
                if (node_lookat != null)
                {
                    string str_lookat = ((XmlText)node_lookat.FirstChild).Data;
                    Point3D lookat = Point3D.FromCsv(str_lookat);
                    //if (lookat != null)
                    //{
                        toReturn.setLookat(lookat);
                    //}
                }
                XmlNode node_exp = camRoot.SelectSingleNode("exposure");
                if (node_exp != null)
                {
                    string str_exp = ((XmlText)node_exp.FirstChild).Data;
                    float exposure = (float)Convert.ToSingle(str_exp);
                    toReturn.setExposure(exposure);
                }

                toReturn.compute_uvw();

                return toReturn;
            }
            else
            {
                Console.WriteLine("Camera type for camera " + camRoot.GetAttribute("id") + " not defined.");
                return toReturn;
            }
        }

        protected class RenderFragmentParameters
        {
            public int h0, h1, v0, v1;
            public World w;
            public int threadNo;
            public Thread t;

            public RenderFragmentParameters(World w_arg, int h0_arg, int h1_arg, int v0_arg, int v1_arg, int tno_arg)
            {
                w = w_arg;
                h0 = h0_arg;
                h1 = h1_arg;
                v0 = v0_arg;
                v1 = v1_arg;
                threadNo = tno_arg;
                t = new Thread(() => w.camera.render_scene_fragment(w, h0, h1, v0, v1, threadNo));
                t.Priority = ThreadPriority.Highest;
            }

            public void Begin()
            {
                t.Start();
            }
        }
    }
}


