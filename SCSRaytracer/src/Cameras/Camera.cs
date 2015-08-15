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
using System.Xml;

namespace RayTracer
{
    public abstract class Camera
    {
        protected Point3D eye;
        protected Point3D lookat;  
        protected Vect3D up;
        protected Vect3D u, v, w;
        protected double exposure_time;
        protected double zoom;

        public Camera()
        {
            this.setUp(new Vect3D(0, 1, 0));
            this.setEye(new Point3D(0, 0, 0));
            this.setExposure(1.0);
            this.setLookat(new Point3D(0, 0, 500));
            this.setExposure(1.0);
            this.setZoom(1.0);
            this.compute_uvw();
        }
        //Getters and setters
        public void setEye(Point3D e) { eye = new Point3D(e); }
        public void setLookat(Point3D l) { lookat = new Point3D(l); }
        public void setUp(Vect3D u) { up = new Vect3D(u); }
        public void setExposure(double e) { exposure_time = e; }
        public void setZoom(double z) { zoom = z; }

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


            vp.s /= zoom;

            List<Thread> threads = new List<Thread>();

            if (numThreads == 2)
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
                threads.Add(new Thread(() => render_scene_fragment(w, (vp.hres / 2), (3 * vp.hres / 4), 0, vp.vres / 2, 2)));
                threads.Add(new Thread(() => render_scene_fragment(w, (3 * vp.hres / 4), vp.hres, 0, vp.vres / 2, 3)));
                threads.Add(new Thread(() => render_scene_fragment(w, 0, (vp.hres / 4), vp.vres / 2, vp.vres, 4)));
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
            bool allDone = false;
            do
            {
                allDone = true;
                foreach (Thread t in threads)
                {
                    if (t.IsAlive)
                    {
                        allDone = false;
                    }
                }
                w.poll_events();

            } while (!allDone);
            //}

        }
        protected abstract void render_scene_fragment(World w, int x1, int x2, int y1, int y2, int threadNo);

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
                    double zoom = Convert.ToDouble(str_zoom);
                    toReturn.setZoom(zoom);
                }

                //Load common attributes afterwards.
                XmlNode node_point = camRoot.SelectSingleNode("point");
                if (node_point != null)
                {
                    string str_point = ((XmlText)node_point.FirstChild).Data;
                    Point3D point = Point3D.FromCsv(str_point);
                    if (point != null)
                    {
                        toReturn.setEye(point);
                    }
                }
                XmlNode node_lookat = camRoot.SelectSingleNode("lookat");
                if (node_lookat != null)
                {
                    string str_lookat = ((XmlText)node_lookat.FirstChild).Data;
                    Point3D lookat = Point3D.FromCsv(str_lookat);
                    if (lookat != null)
                    {
                        toReturn.setLookat(lookat);
                    }
                }
                XmlNode node_exp = camRoot.SelectSingleNode("exposure");
                if (node_exp != null)
                {
                    string str_exp = ((XmlText)node_exp.FirstChild).Data;
                    double exposure = Convert.ToDouble(str_exp);
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
    }
}
