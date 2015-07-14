using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public abstract class Camera
    {
        protected Point3D eye;
        protected Point3D lookat;
        protected Vect3D up;
        protected Vect3D u, v, w;
        protected double exposure_time;

        //Getters and setters
        public void setEye(Point3D e) { eye = new Point3D(e); }
        public void setLookat(Point3D l) { lookat = new Point3D(l); }
        public void setUp(Vect3D u) { up = new Vect3D(u); }
        public void setExposure(double e) { exposure_time = e; }

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
    }
}
