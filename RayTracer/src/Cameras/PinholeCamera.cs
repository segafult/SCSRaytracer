using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class PinholeCamera : Camera
    {
        private float d; //Distance between pinhole and viewplane
        private float zoom; //Zoom factor

        public void setVdp(float distance)
        {
            d = distance;
        }
        public void setZoom(float z)
        {
            zoom = z;
        }

        public override void render_scene(World w)
        {
            RGBColor L;
            ViewPlane vp = w.vp;
            Ray ray = new Ray(eye,new Vect3D(0,0,0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space
            exposure_time = 1.0;

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
                        pp.x = w.vp.s * (column - 0.5 * vp.hres + sp.x);
                        pp.y = w.vp.s * (row - 0.5 * vp.vres + sp.y);
                        ray.direction = ray_direction(pp);
                        L = L + w.tracer.trace_ray(ray, depth);
                    }

                    L /= vp.numSamples;
                    L *= exposure_time;
                    w.display_pixel(row, column, L);
                }
            }
        }
        private Vect3D ray_direction(Point2D p)
        {
            Vect3D dir = p.x * u + p.y * v - d * w;
            dir.normalize();
            return dir;
        }
    }
}
