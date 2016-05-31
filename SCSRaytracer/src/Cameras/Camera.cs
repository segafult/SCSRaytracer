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
    /// <summary>
    /// Base Camera class. Provides framework and default constructor. Cameras work in color space, and treat pixels
    /// as colors rather than ray interesections.
    /// </summary>
    abstract class Camera
    {
        protected Point3D _eye;
        protected Point3D _lookAt;
        protected Vect3D _up;
        protected float _exposureTime;
        protected float _zoom;
        protected Vect3D u, v, w;
        protected ConcurrentQueue<RenderFragmentParameters> taskQueue;

        //Accessors
        public Point3D Eye
        {
            set
            {
                _eye = new Point3D(value);
            }
        }
        public Point3D LookAt
        {
            set
            {
                _lookAt = new Point3D(value);
            }
        }
        public Vect3D Up
        {
            set
            {
                _up = new Vect3D(value);
            }
        }
        public float Exposure
        {
            set
            {
                _exposureTime = value;
            }
        }
        public float Zoom
        {
            set
            {
                _zoom = value;
            }
        }

        public Camera()
        {
            this.Up = new Vect3D(0, 1, 0);
            this.Eye = new Point3D(0, 0, 0);
            this.LookAt = new Point3D(0, 0, 500);
            this.Exposure = 1.0f;
            this.Zoom = 1.0f;
            this.compute_uvw();
        }



        public void compute_uvw() 
        {
            _up = new Vect3D(0, 1, 0);
            w = _eye - _lookAt;
            w.Normalize();
            u = _up ^ w;
            u.Normalize();
            v = w ^ u;
        }

        /// <summary>
        /// Renders the world on a single thread [deprecated, use RenderSceneMultiThreaded]
        /// </summary>
        /// <param name="world">World reference</param>
        public abstract void RenderScene(World world);

        /// <summary>
        /// Renders a frame for the current world across a given number of threads
        /// </summary>
        /// <param name="world">Current world with camera</param>
        /// <param name="numThreads">Number of threads to split workload across</param>
        public virtual void RenderSceneMultithreaded(World world, int numThreads)
        {
            // local ref for viewplane for minimized member accesses
            ViewPlane vp = world.CurrentViewPlane;

            // set zoom for frame zero to ensure animation does not cause increasing zoom
            if(GlobalVars.frameno == 0)
                vp.PixelSize /= _zoom;


            // list of all active threads, for spinwait
            List<RenderFragmentParameters> threads = new List<RenderFragmentParameters>();
            // concurrent queue for thread safe dequeue of render chunks
            taskQueue = new ConcurrentQueue<RenderFragmentParameters>();

            //Find out how many chunks the screen can be divided into;
            int hChunks = (int)Math.Floor((decimal)vp.HorizontalResolution / (decimal)GlobalVars.FRAGMENT_SIZE);
            int vChunks = (int)Math.Floor((decimal)vp.VerticalResolution / (decimal)GlobalVars.FRAGMENT_SIZE);
            //Find out the size of the boundaries which do not fit into chunks
            int hRemaining = vp.HorizontalResolution % GlobalVars.FRAGMENT_SIZE;
            int vRemaining = vp.VerticalResolution % GlobalVars.FRAGMENT_SIZE;

            int threadNo = 0; //Label for each thread created

            //Queue up render chunks
            int hor0, hor1, vert0, vert1;
            RenderFragmentParameters parameters;
            for(int v = 0; v < vChunks; v++)
            {
                for(int h = 0; h < hChunks; h++)
                {
                    //Queue up threads
                    hor0 = GlobalVars.FRAGMENT_SIZE * h;
                    hor1 = GlobalVars.FRAGMENT_SIZE * (h + 1);
                    vert0 = GlobalVars.FRAGMENT_SIZE * v;
                    vert1 = GlobalVars.FRAGMENT_SIZE * (v + 1);
                    parameters = new RenderFragmentParameters(world, hor0, hor1, vert0, vert1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                    threadNo++;
                }
                //Add in additional fragments for the right screen edge if resolution isn't nicely
                //divisible
                if(hRemaining != 0)
                {
                    hor0 = GlobalVars.FRAGMENT_SIZE * hChunks;
                    hor1 = vp.HorizontalResolution;
                    vert0 = GlobalVars.FRAGMENT_SIZE * v;
                    vert1 = GlobalVars.FRAGMENT_SIZE * (v + 1);
                    parameters = new RenderFragmentParameters(world, hor0, hor1, vert0, vert1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
            }
            //Add in additional fragments for the top edge if resolution isn't nicely divisible
            if(vRemaining != 0)
            {
                for(int h = 0; h < hChunks; h++)
                {
                    hor0 = GlobalVars.FRAGMENT_SIZE * h;
                    hor1 = GlobalVars.FRAGMENT_SIZE * (h + 1);
                    vert0 = GlobalVars.FRAGMENT_SIZE * vChunks;
                    vert1 = vp.VerticalResolution;
                    parameters = new RenderFragmentParameters(world, hor0, hor1, vert0, vert1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
                //Add in corner edge fragment if the right edge of the screen wasn't nicely divisible
                if (hRemaining != 0)
                {
                    hor0 = GlobalVars.FRAGMENT_SIZE * hChunks;
                    hor1 = vp.HorizontalResolution;
                    vert0 = GlobalVars.FRAGMENT_SIZE * vChunks;
                    vert1 = vp.VerticalResolution;
                    parameters = new RenderFragmentParameters(world, hor0, hor1, vert0, vert1, threadNo);
                    threads.Add(parameters);
                    taskQueue.Enqueue(parameters);
                }
            }

            // dequeue either the provided number of threads, or the total number of threads, whichever is smaller
            int numThreadsToDequeue = (numThreads < threadNo) ? numThreads : threadNo;

            // dequeue the initial thread count of threads
            RenderFragmentParameters[] initialThreads = new RenderFragmentParameters[numThreadsToDequeue];
            for (int i = 0; i < numThreadsToDequeue; i++)
            {
                while(!taskQueue.TryDequeue(out initialThreads[i])) { // ensure every dequeue succeeds
                }
            }

            // begin rendering in all dequeued threads
            foreach(RenderFragmentParameters renderFragment in initialThreads) 
            {
                renderFragment.Begin();
            }

            //Set the thread priority for main thread to low
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            //Spinwait for all threads
            bool allDone = false;
            do
            {
                allDone = true;
                foreach (RenderFragmentParameters thread in threads)
                {
                    if (thread.thread.IsAlive)
                    {
                        allDone = false;
                    }
                }
                world.PollEvents();

            } while (!allDone);

            // update liveimage
            world.LiveView.LiveImage = new SFML.Graphics.Image((uint)vp.HorizontalResolution, (uint)vp.VerticalResolution, world.RenderImage);
            
        }

        /// <summary>
        /// Renders a single rectangular chunk of the scene
        /// </summary>
        /// <param name="worldRef">Reference to the world</param>
        /// <param name="xCoord1">Smallest x coordinate</param>
        /// <param name="xCoord2">Largest x coordinate</param>
        /// <param name="yCoord1">Smallest y coordinate</param>
        /// <param name="yCoord2">Largest y coordinate</param>
        /// <param name="threadNum">Thread number</param>
        public abstract void RenderSceneFragment(World worldRef, int xCoord1, int xCoord2, int yCoord1, int yCoord2, int threadNum);


        /// <summary>
        /// Called at end of RenderSceneFragment. Attempts dequeue of next render fragment if another left in queue.
        /// </summary>
        public void DequeueNextRenderFragment()
        {
            RenderFragmentParameters renderFragment;
            while(!taskQueue.TryDequeue(out renderFragment)) //Keep trying to dequeue if concurrent queue is locked
            {
                //Check that the queue has something in it. If it's empty then no need to dequeue next render task
                if (taskQueue.IsEmpty) break;
            }
            if (renderFragment != null) renderFragment.Begin();
        }


        /// <summary>
        /// Load function for XML file
        /// </summary>
        /// <param name="camRoot">Root element of camera tag</param>
        /// <returns>A fully constructed Camera</returns>
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
                    toReturn.Zoom = zoom;
                }

                //Load common attributes afterwards.
                XmlNode node_point = camRoot.SelectSingleNode("point");
                if (node_point != null)
                {
                    string str_point = ((XmlText)node_point.FirstChild).Data;
                    Point3D point = Point3D.FromCsv(str_point);
                    //if (point != null)
                    //{
                        toReturn.Eye = point;
                    //}
                }
                XmlNode node_lookat = camRoot.SelectSingleNode("lookat");
                if (node_lookat != null)
                {
                    string str_lookat = ((XmlText)node_lookat.FirstChild).Data;
                    Point3D lookat = Point3D.FromCsv(str_lookat);
                    //if (lookat != null)
                    //{
                        toReturn.LookAt = lookat;
                    //}
                }
                XmlNode node_exp = camRoot.SelectSingleNode("exposure");
                if (node_exp != null)
                {
                    string str_exp = ((XmlText)node_exp.FirstChild).Data;
                    float exposure = (float)Convert.ToSingle(str_exp);
                    toReturn.Exposure = exposure;
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

        /// <summary>
        /// Locally used utility class for storage of parameters and thread handle for dequeue
        /// </summary>
        protected class RenderFragmentParameters
        {
            public int horizontal0, horizontal1, vertical0, vertical1;
            public World worldRef;
            public int threadNo;
            public Thread thread;

            /// <summary>
            /// Parameterized constructor for a given render fragment
            /// </summary>
            /// <param name="w_arg">Reference to world, passed to RenderSceneFragment</param>
            /// <param name="h0_arg">Smallest horizontal render point</param>
            /// <param name="h1_arg">Largest horizontal render point</param>
            /// <param name="v0_arg">Smallest horizontal render point</param>
            /// <param name="v1_arg">Largest hozitontal render point</param>
            /// <param name="tno_arg">Thread id</param>
            public RenderFragmentParameters(World w_arg, int h0_arg, int h1_arg, int v0_arg, int v1_arg, int tno_arg)
            {
                worldRef = w_arg;
                horizontal0 = h0_arg;
                horizontal1 = h1_arg;
                vertical0 = v0_arg;
                vertical1 = v1_arg;
                threadNo = tno_arg;
                // construct new thread using lambda expression for parameterized thread start
                thread = new Thread(() => worldRef.Camera.RenderSceneFragment(worldRef, horizontal0, horizontal1, vertical0, vertical1, threadNo));
                thread.Priority = ThreadPriority.Highest;
            }

            public void Begin()
            {
                thread.Start();
            }
        }
    }
}


