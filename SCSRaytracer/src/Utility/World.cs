//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Runtime.CompilerServices;


namespace SCSRaytracer
{
    /// <summary>
    /// Class containing references to all world items
    /// </summary>
    sealed class World
    {
        private ViewPlane _viewPlane; // viewplane
        private RGBColor _backgroundColor; // background color
        private Tracer _tracer; // tracer to be used in rendering scene, default Whitted

        private List<RenderableObject> _renderList;  //Renderlist is the actual list of objects to render
        private List<RenderableObject> _objectList;  //Objectlist isn't rendered, and instead serves to hold objects to be instanced
        private List<Light> _lightList; // List of lights
        private List<Material> _materialList; // List of all materials to be used in scene rendering

        private AmbientLight _ambientLight;   // Ambient lighting is treated differently from raytraced sources, keep outside list
        private Camera _camera; // Camera reference
        private LiveViewer _liveView; // Live viewer window
        private byte[] _renderImage; // 1D byte array for image rendering

        // accessors
        public ViewPlane CurrentViewPlane
        {
            get
            {
                return _viewPlane;
            }
        }
        public RGBColor CurrentBackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
        }
        public Tracer CurrentTracer
        {
            get
            {
                return _tracer;
            }
            set
            {
                _tracer = value;
            }
        }
        public List<RenderableObject> RenderList
        {
            get
            {
                return _renderList;
            }
        }
        public List<RenderableObject> ObjectList
        {
            get
            {
                return _objectList;
            }
        }
        public List<Light> LightList
        {
            get
            {
                return _lightList;
            }
        }
        public List<Material> MaterialList
        {
            get
            {
                return _materialList;
            }
        }
        public AmbientLight AmbientLight
        {
            get
            {
                return _ambientLight;
            }
            set
            {
                _ambientLight = value;
            }
        }
        public Camera Camera
        {
            get
            {
                return _camera;
            }
            set
            {
                _camera = value;
            }
        }
        public LiveViewer LiveView
        {
            get
            {
                return _liveView;
            }
        }
        public byte[] RenderImage
        {
            get
            {
                return _renderImage;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public World ()
        {
            // give world a default viewplane and ambient light
            _viewPlane = new ViewPlane();
            _ambientLight = new AmbientLight();

            // set up scene lists
            _renderList = new List<RenderableObject>();
            _objectList = new List<RenderableObject>();
            _lightList = new List<Light>();
            _materialList = new List<Material>();
            
        }

        /// <summary>
        /// Appends given render object to the end of the scene list
        /// </summary>
        /// <param name="o">Object to add</param>
        public void AddObjectToScene(RenderableObject o)
        {
            _renderList.Add(o);
        }

        /// <summary>
        /// Appends object to the end of the instantiable objects list
        /// </summary>
        /// <param name="o">Object to add</param>
        public void AddObject(RenderableObject o)
        {
            _objectList.Add(o);
        }

        /// <summary>
        /// Appends a light to the end of the lights list
        /// </summary>
        /// <param name="l">Light to add</param>
        public void AddLight(Light l)
        {
            _lightList.Add(l);
        }

        /// <summary>
        /// Appends a material to the end of the materials list
        /// </summary>
        /// <param name="m">Material to add</param>
        public void AddMaterial(Material m)
        {
            _materialList.Add(m);
        }

        /// <summary>
        /// Step 1 in rendering pipeline.
        /// Generic intersection detection for objectList called by a Tracer, returns ShadeRec describing intersection.
        /// </summary>
        /// <param name="ray">Ray for intersection calculation</param>
        /// <returns>ShadeRec object with all relevant information about object that was intersected for generating
        /// pixel data</returns>
        public ShadeRec HitObjects(Ray ray)
        {
            ShadeRec sr = new ShadeRec(this); // create shaderec
            
            // set normal and local hit point to non-null values
            Normal normal = new Normal();
            Point3D local_hit_point = new Point3D();

            float t = GlobalVars.K_HUGE_VALUE - 1.0f; // t value for corrent object
            float tmin = GlobalVars.K_HUGE_VALUE; //  current lowest t value
            int num_objects = _renderList.Count; // store count of objects to minimize unecessary member access
            Material closestmat = null; // material reference for closest object

            //Find the closest intersection point along the given ray
            for(int i = 0; i < num_objects; i++)
            {
                // Intersect each object in render list
                // First term evaluated first, therefore (t < tmin) will be doing a comparison of t value
                // of present object vs minimum t value.
                if (_renderList[i].Hit(ray, ref t, ref sr) && (t < tmin))
                {
                    sr.HitAnObject = true; // at least one intersection has occurred

                    // store temporary references to information about current object with minimum t
                    tmin = t;
                    closestmat = sr.ObjectMaterial;
                    sr.HitPoint = ray.Origin + t * ray.Direction; // calculate hit point in world space by adding t*direction to origin
                    normal = sr.Normal;
                    local_hit_point = sr.HitPointLocal;
                }
            }

            //If we hit an object, store local vars for closest object with ray intersection in sr before returning
            if(sr.HitAnObject)
            {
                sr.TMinimum = tmin;
                sr.Normal = normal;
                sr.HitPointLocal = local_hit_point;
                sr.ObjectMaterial = closestmat;
            }

            return (sr);
        }

        /// <summary>
        /// Builds the world
        /// </summary>
        public void Build()
        {
            _backgroundColor = GlobalVars.COLOR_BLACK;

            if (GlobalVars.inFile != null)
            {
                // Attempt to open an input file
                XMLProcessor sceneLoader = new XMLProcessor(GlobalVars.inFile, this);

                // Load materials from XML file, if verbose output enabled provide debug info in console
                sceneLoader.LoadMaterials();
                if (GlobalVars.verbose)
                {
                    Console.WriteLine("Materials loaded:");
                    foreach (Material m in _materialList)
                    {
                        Console.WriteLine(m.ToString());
                    }
                }

                // Load objects for instancing from XML file, if verbose output enabled provide debug info in console
                sceneLoader.LoadObjects();
                if (GlobalVars.verbose)
                {
                    Console.WriteLine("Object definitions loaded:");
                    foreach (RenderableObject o in _objectList)
                    {
                        Console.WriteLine(o.ToString());
                    }
                }

                // Finally, load the scene from XML file, if verbose output enabled provide debug info in console
                sceneLoader.LoadWorld();
                if (GlobalVars.verbose)
                {
                    Console.WriteLine("Scene loaded!");
                    foreach (RenderableObject o in _renderList)
                    {
                        Console.WriteLine(o.ToString());
                    }
                }
                /*
                //Sphere mysphere = new Sphere(new Point3D(0, 0, 0), 1000);
                //mysphere.setMaterial(getMaterialById("myreflective"));
                //add_Object(mysphere);
                //((ThinLensCamera)camera).set_sampler(vp.vpSampler);
                
                UniformGrid mygrid = new UniformGrid();

                //Plane groundplane = new Plane(new Point3D(0, -100, 0), new Normal(0, 1, 0));
                //groundplane.setMaterial(new DebugCheckerboard());
                //add_Object(groundplane);

                SphericalMapper sphere_map = new SphericalMapper();
                RectangularMapper rect_map = new RectangularMapper();
                Image my_image = new Image();
                my_image.LoadFromFile("E:\\earth.jpg");

                ImageTexture my_tex = new ImageTexture();
                my_tex.MapType = rect_map;
                my_tex.Image = my_image;

                MatteShader mymatte = new MatteShader();
                //mymatte.setCr(my_tex);
                mymatte.Texture = my_tex;
                //mymatte.setReflectivity(0.95f);
                //mymatte.setKa(0.9f);
                //PhongShader myshader = (PhongShader)getMaterialById("myphong");
                //myshader.setCd(my_tex);
                //Sphere mysphere = new Sphere(new Point3D(0, 0, 0), 20);
                //mysphere.setMaterial(mymatte);

                //Instance myinstance = new Instance(mysphere);
                //myinstance.scale(new Vect3D(5, 5, 5));
                //myinstance.rotate(new Vect3D(23.5f, 90, 0));
                //mygrid.add_object(myinstance);
                //add_Object(myinstance);

                Image my_skybox = new Image();
                my_skybox.LoadFromFile("E:\\skybox.jpg");

                my_tex = new ImageTexture();
                my_tex.MapType = sphere_map;
                my_tex.Image = my_skybox;

                
                MatteShader skymatte = new MatteShader();
                skymatte.Texture = my_tex;
                skymatte.AmbientReflectionCoefficient = 1.0f;

                Sphere skybox = new Sphere(new Point3D(0, 10, 0),1000);
                skybox.Material = skymatte;
                //mygrid.add_object(skybox);
                AddObjectToScene(skybox);

                //mygrid.setup_cells();
                //add_Object(mygrid);

                Mesh dragon = new Mesh();
                dragon.loadFromFile("E:\\dragon.off", true);
                dragon.setup_cells();
                dragon.Material = new PhongShader();
                Instance myInstance = new Instance(dragon);
                myInstance.Scale(100, 100, 100);
                myInstance.Rotate(90, 40, 0);
                myInstance.Material = new PhongShader();
                AddObjectToScene(myInstance);
                */
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
        /// Animate is called on every frame. Allows for custom animation instructions to be inserted.
        /// </summary>
        public void Animate()
        {
            
        }

        /// <summary>
        /// Sets up rendering field
        /// </summary>
        /// <param name="hres">Horizontal resolution</param>
        /// <param name="vres">Vertical resolution</param>
        public void OpenWindow(int hres, int vres)
        {
            // Create live viewer window, and allocate memory for the live view image
            _liveView = new LiveViewer(this);
            _renderImage = new byte[hres * vres * 4];
            _liveView.SetUpLiveView();
        }

        /// <summary>
        /// Performs the required colorspace conversions
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="pixel_color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DisplayPixel(int row, int column, RGBColor pixel_color)
        {
            RGBColor disp_color = new RGBColor(pixel_color.Values * 255.0f);
            Vector3 cols = disp_color.clamp().Values;
            int x = column;
            int y = _viewPlane.VerticalResolution-1-row;
            //live_view.live_image.SetPixel((uint)column, (uint)(vp.vres-1-row), new SFML.Graphics.Color(r, g, b));
            _renderImage[(y * 4 * _viewPlane.HorizontalResolution) + (x * 4)] = (byte)cols.X;
            _renderImage[(y * 4 * _viewPlane.HorizontalResolution) + (x * 4) + 1] = (byte)cols.Y;
            _renderImage[(y * 4 * _viewPlane.HorizontalResolution) + (x * 4) + 2] = (byte)cols.Z;
            _renderImage[(y * 4 * _viewPlane.HorizontalResolution) + (x * 4) + 3] = 255;
        }

        /// <summary>
        /// Utitilty function, provides handle for window thread for processing window events
        /// </summary>
        /// <returns>Handle for thread that the live view window is displayed on</returns>
        public Thread GetWindowThread()
        {
            return _liveView.RenderThread;
        }

        /// <summary>
        /// Polls mouse and keyboard events for live view window
        /// </summary>
        public void PollEvents()
        {
            _liveView.PollEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void SaveDisplayedImage(string path)
        {
            _liveView.LiveImage.SaveToFile(path);
        }

        /// <summary>
        /// Perform lookup of material in materials list by material id
        /// </summary>
        /// <param name="idarg">Material ID to look up</param>
        /// <returns>A default matte shader in the event that material does not exist, material with ID=idarg if it exists</returns>
        public Material GetMaterialByID(string idarg)
        {
            //idarg will be null if no node is returned by the XmlProcessor
            if (idarg == null)
            {
                return new MatteShader();
            }
            else
            {
                int numMats = _materialList.Count;

                // Search for material and return it
                for (int i = 0; i < numMats; i++)
                {
                    if (_materialList[i].id.Equals(idarg))
                    {
                        return _materialList[i];
                    }
                }
                // If not found, return a default
                return new MatteShader();
            }
        }

        /// <summary>
        /// Performs lookup of an object in the objects list using a provided string for object id
        /// </summary>
        /// <param name="objarg">Object ID</param>
        /// <returns>Null if object not found, object with ID of objarg if found</returns>
        public RenderableObject GetObjectByID(string objarg)
        {
            // return null for null argument
            if(objarg == null)
            {
                return null;
            }
            else
            {
                int numObjs = _objectList.Count;

                // Search for object and return it
                for(int i = 0; i < numObjs; i++)
                {
                    if (_objectList[i].id.Equals(objarg))
                    {
                        return _objectList[i];
                    }
                }
                // If not found, return a default
                return null;
            }
        }
    } 
}
