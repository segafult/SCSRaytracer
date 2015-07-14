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
        public AmbientLight ambientLight;
        public Camera camera;

        public Bitmap drawPlan;

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
            vp.set_hres(800);
            vp.set_vres(600);
            vp.set_pixel_size(1.0F);
            vp.set_gamma(1.0F);
            vp.set_samples(4);
            RegularSampler mySampler = new RegularSampler(vp.numSamples);
            mySampler.generate_samples();
            vp.set_sampler(mySampler);

            bg_color = GlobalVars.color_black;
            tracer = new RayCaster(this);

            PinholeCamera pinhole_ptr = new PinholeCamera();
            pinhole_ptr.setEye(new Point3D(0, 0, 500));
            pinhole_ptr.setLookat(new Point3D(-5, 0, 0));
            pinhole_ptr.setVdp(850.0F);
            pinhole_ptr.setZoom(2.0F);
            pinhole_ptr.compute_uvw();
            set_camera(pinhole_ptr);

            PointLight light_ptr = new PointLight(new Point3D(100,50,150));
            light_ptr.setIntensity(3.0);
            add_Light(light_ptr);

            MatteShader matte_ptr = new MatteShader();
            matte_ptr.setKa(0.25F);
            matte_ptr.setKd(0.65F);
            matte_ptr.setCd(new RGBColor(1, 1, 0));
            Sphere sphere_ptr = new Sphere(new Point3D(10, -5, 0), 27);
            sphere_ptr.setMaterial(matte_ptr);
            add_Object(sphere_ptr);

            matte_ptr = new MatteShader();
            matte_ptr.setKa(0.25f);
            matte_ptr.setKd(0.65f);
            matte_ptr.setCd(new RGBColor(1, 0, 0));
            sphere_ptr = new Sphere(new Point3D(-27, -12, 15), 20);
            sphere_ptr.setMaterial(matte_ptr);
            add_Object(sphere_ptr);

            matte_ptr = new MatteShader();
            matte_ptr.setKa(0.25f);
            matte_ptr.setKd(0.65f);
            matte_ptr.setCd(new RGBColor(0, 1, 0));
            sphere_ptr = new Sphere(new Point3D(30, -10, -10), 15);
            sphere_ptr.setMaterial(matte_ptr);
            add_Object(sphere_ptr);

            matte_ptr = new MatteShader();
            matte_ptr.setKa(0.25F);
            matte_ptr.setKd(0.65F);
            matte_ptr.setCd(new RGBColor(0.5, 0.5, 0.5));
            Plane plane_ptr = new Plane(new Point3D(10,-32 ,0),new Normal(0,1,0));
            plane_ptr.setMaterial(matte_ptr);
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
        /// Performs the required colorspace conversions
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="pixel_color"></param>
        public void display_pixel(int row, int column, RGBColor pixel_color)
        {
            drawPlan.SetPixel(column, row, Color.FromArgb(255, (int)(pixel_color.r * 250), (int)(pixel_color.g * 250), (int)(pixel_color.b * 250)));
        }
    }
}
