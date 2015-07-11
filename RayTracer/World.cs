using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
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

        List<RenderableObject> renderList;
        public List<Light> lightList;

        private Bitmap drawPlan;
        private PictureBox picBox;
        private Form renderWindow;

        public World ()
        {
            vp = new ViewPlane();
            renderList = new List<RenderableObject>();
            lightList = new List<Light>();   
        }

        /// <summary>
        /// Appends given render object to the end of the scene list
        /// </summary>
        /// <param name="o">Object to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void add_Object(RenderableObject o)
        {
            renderList.Add(o);
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
        /// Builds the world
        /// </summary>
        public void build()
        {
            vp.set_hres(640);
            vp.set_vres(480);
            vp.set_pixel_size((float)1.0);
            vp.set_gamma((float)1.0);

            bg_color = GlobalVars.color_black;
            tracer = new MultipleObjects(this);

            Sphere sphere_ptr = new Sphere(new Point3D(0, -25, 0), 80);
            sphere_ptr.color = new RGBColor(1, 0, 0);

            add_Object(sphere_ptr);

            sphere_ptr = new Sphere(new Point3D(0, 30, 0), 60);
            sphere_ptr.color = new RGBColor(1, 1, 0);

            //add_Object(sphere_ptr);

            //Plane plane_ptr = new Plane(new Point3D(0, 0, 0), new Normal(0, 1, 1));
            //plane_ptr.color = new RGBColor(0, 0.3, 0);
            //add_Object(plane_ptr);

            lightList.Add(new WorldLight(new Vect3D(0.5, -1, 1)));
        }

        /// <summary>
        /// Main render loop
        /// </summary>
        public void render_scene()
        {
            RGBColor pixel_color;
            Ray myRay = new Ray();
            double zw = 100.0;
            double x, y;
            open_window(vp.hres, vp.vres);

            myRay.direction = new Vect3D(0, 0, -1);
            //for(int numloop =0;numloop<100;numloop++) { 
            for (int r = 0; r < vp.vres; r++)
            {
                for (int c = 0; c < vp.hres; c++)
                {
                    x = vp.s * (c - 0.5 * (vp.hres - 1.0));
                    y = vp.s * (r - 0.5 * (vp.vres - 1.0));
                    myRay.origin = new Point3D(x, y, zw);
                    pixel_color = tracer.trace_ray(myRay);
                    display_pixel(r, c, pixel_color);
                 }
            }
            //}
            drawPlan.Save("E:\\test.bmp");
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
        /// Performs the required colorspace conversions
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="pixel_color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void display_pixel(int row, int column, RGBColor pixel_color)
        {
            pixel_color = pixel_color ^ 2.2;
            pixel_color.clamp();
            drawPlan.SetPixel(column, row, Color.FromArgb(255, (int)(pixel_color.r * 127), (int)(pixel_color.g * 127), (int)(pixel_color.b * 127)));
        }
    }
}
