//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;

namespace SCSRaytracer
{
    sealed class XMLProcessor
    {
        private XmlDocument _sceneXML;
        private XmlReader _sceneReader;
        private XmlNode _rootNode;
        private World _world;

        public XMLProcessor(World worldref)
        {
            _sceneXML = new XmlDocument();
            _world = worldref;
        }
        public XMLProcessor(XmlReader doc, World worldref)
        {
            _sceneXML = new XmlDocument();
            _sceneReader = doc;
            _sceneXML.Load(_sceneReader);
            _world = worldref;
        }

        /// <summary>
        /// Constructor, requires filename and handle for the world
        /// </summary>
        /// <param name="filename">File to open</param>
        /// <param name="worldref">World reference</param>
        public XMLProcessor(string filename, World worldref)
        {
            try {
                _sceneXML = new XmlDocument();
                _sceneReader = new XmlTextReader(filename);
                _sceneXML.Load(_sceneReader);
                _rootNode = _sceneXML.DocumentElement;
                _world = worldref;
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Loads all materials provided in the header section of provided SCSML document
        /// </summary>
        public void LoadMaterials()
        {
            // Attempt to select all nodes within the <materials> section of the header.
            try
            {
                XmlNodeList mats = _rootNode.SelectNodes("materials");
                if (mats == null)
                    throw new XmlException("Invalid SCSML: No material tags present in XML document.");
                foreach (XmlNode matRoot in mats)
                {
                    //Load materials
                    this.LoadMatte(matRoot);
                    this.LoadPhong(matRoot);
                    this.LoadReflective(matRoot);
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Loads all objects provided in header section of provided SCSML document
        /// </summary>
        public void LoadObjects()
        {
            try
            {
                XmlNodeList objects = _rootNode.SelectNodes("objects");
                if (objects == null)
                    throw new XmlException("Invalid SCSML: No object tags present in XML document.");
                foreach (XmlNode objectRoot in objects) {
                    
                    //Get the list of renderables
                    XmlNodeList renderables = objectRoot.SelectNodes("renderable");

                    foreach(XmlElement rendRoot in renderables)
                    {
                        RenderableObject rend = RenderableObject.LoadRenderableObject(rendRoot);
                        if (rend != null)
                            _world.AddObject(rend);
                    }
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void LoadWorld()
        {
            try
            {
                XmlNode scene = _rootNode.SelectSingleNode("scene");
                if (scene == null)
                    throw new XmlException("No scene defined!");

                //Load world according to parameters of scene
                this.SetupWorldParameters((XmlElement)scene);
                this.LoadLights((XmlElement)scene);

                XmlNodeList renderables = scene.SelectNodes("renderable");
                foreach(XmlElement renderable in renderables)
                {
                    RenderableObject myrend = RenderableObject.LoadRenderableObject(renderable);
                    if (myrend != null)
                        _world.AddObjectToScene(myrend);
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //-------------------------------------------------------------------------------------------------------
        //   MATERIAL LOADERS
        //-------------------------------------------------------------------------------------------------------
        private void LoadMatte(XmlNode matRoot)
        {
            XmlNodeList currentContext = matRoot.SelectNodes("matte");

            foreach (XmlNode definition in currentContext)
            {
                //Filter all material definitions that don't have an id
                if (((XmlElement)definition).HasAttribute("id"))
                {

                    MatteShader matte = new MatteShader();
                    matte.id = ((XmlElement)definition).GetAttribute("id");


                    try {
                        //Load Ka value if provided
                        XmlNode ka = definition.SelectSingleNode("ka");
                        if (ka != null)
                        {
                            float kaDouble = Convert.ToSingle(((XmlText)ka.FirstChild).Data);
                            matte.AmbientReflectionCoefficient = kaDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            matte.DiffuseReflectionCoefficient = kdDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;

							matte.ColorDiffuse = new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr));

                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    _world.AddMaterial(matte);
                }
                else
                {
                    throw new XmlException("No id provided for matte shader definition.");
                }
            }
        }
        private void LoadPhong(XmlNode matRoot)
        {
            XmlNodeList currentContext = matRoot.SelectNodes("phong");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all material definitions that don't have an id
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    PhongShader phong = new PhongShader();
                    phong.id = ((XmlElement)definition).GetAttribute("id");

                    try {
                        //Load Ka value if provided
                        XmlNode ka = definition.SelectSingleNode("ka");
                        if (ka != null)
                        {
                            float kaDouble = Convert.ToSingle(((XmlText)ka.FirstChild).Data);
                            phong.AmbientReflectionCoefficient = kaDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            phong.DiffuseReflectionCoefficient = kdDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            phong.Color = new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            float ksDouble = Convert.ToSingle(((XmlText)ks.FirstChild).Data);
                            phong.SpecularReflectionCoefficient = ksDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            float expDouble = Convert.ToSingle(((XmlText)exp.FirstChild).Data);
                            phong.PhongExponent = expDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    _world.AddMaterial(phong);
                }
                else
                {
                    Console.WriteLine("Warning: Phong shader definition lacks an id handle and will be skipped.");
                }
            }
        }
        private void LoadReflective(XmlNode matRoot)
        {
            XmlNodeList currentContext = matRoot.SelectNodes("reflective");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all material definitions that don't have an id
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    ReflectiveShader reflective = new ReflectiveShader();
                    reflective.id = ((XmlElement)definition).GetAttribute("id");

                    try {
                        //Load Ka value if provided
                        XmlNode ka = definition.SelectSingleNode("ka");
                        if (ka != null)
                        {
                            float kaDouble = Convert.ToSingle(((XmlText)ka.FirstChild).Data);
                            reflective.AmbientReflectionCoefficient = kaDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            reflective.AmbientReflectionCoefficient = kdDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            reflective.Color = (new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            float ksDouble = Convert.ToSingle(((XmlText)ks.FirstChild).Data);
                            reflective.SpecularReflectionCoefficient = ksDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            float expDouble = Convert.ToSingle(((XmlText)exp.FirstChild).Data);
                            reflective.PhongExponent = expDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kr if provided
                        XmlNode kr = definition.SelectSingleNode("kr");
                        if (kr != null)
                        {
                            float krDouble = Convert.ToSingle(((XmlText)kr.FirstChild).Data);
                            reflective.ReflectiveReflectionCoefficient = krDouble;
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }


                    _world.AddMaterial(reflective);
                }
                else
                {
                    Console.WriteLine("Warning: Reflective shader definition lacks an id handle and will be skipped.");
                }
            }
        }
        //-------------------------------------------------------------------------------------------------------
        //   END MATERIAL LOADERS
        //-------------------------------------------------------------------------------------------------------

        private void SetupWorldParameters(XmlElement scene)
        {
            //Setup world according to default parameters before doing anything.
            _world.CurrentViewPlane.HorizontalResolution = GlobalVars.H_RES;
            _world.CurrentViewPlane.VerticalResolution = GlobalVars.V_RES;
            _world.CurrentViewPlane.Gamma = 1.0f;
            _world.CurrentViewPlane.PixelSize = 1.0f;
            _world.CurrentViewPlane.MaximumRenderDepth = 5;
            _world.CurrentViewPlane.NumSamples = 1;
            _world.CurrentViewPlane.ViewPlaneSampler = new RegularSampler(_world.CurrentViewPlane.NumSamples);
            _world.CurrentViewPlane.ViewPlaneSampler.GenerateSamples();
            _world.Camera = new PinholeCamera();
            _world.CurrentTracer = new RayCaster(_world);

            //Good to go, begin reading in parameters as provided
            string str_hres = scene.GetAttribute("hres");
            if (!str_hres.Equals(""))
            {
                _world.CurrentViewPlane.HorizontalResolution = Convert.ToInt32(str_hres);
            }
            string str_vres = scene.GetAttribute("vres");
            if (!str_vres.Equals(""))
            {
                _world.CurrentViewPlane.VerticalResolution = Convert.ToInt32(str_vres);
            }
            string str_gamma = scene.GetAttribute("gamma");
            if (!str_gamma.Equals(""))
            {
                _world.CurrentViewPlane.Gamma = Convert.ToSingle(str_gamma);
            }
            string str_pixelsize = scene.GetAttribute("px");
            if (!str_pixelsize.Equals(""))
            {
                _world.CurrentViewPlane.PixelSize = Convert.ToSingle(str_pixelsize);
            }
            string str_renderdepth = scene.GetAttribute("renderdepth");
            if (!str_pixelsize.Equals(""))
            {
                _world.CurrentViewPlane.MaximumRenderDepth = Convert.ToInt32(str_renderdepth);
            }
            string str_samples = scene.GetAttribute("multisample");
            if (!str_samples.Equals(""))
            {
                //Check if the number of samples is a perfectly square number
                int int_samples = Convert.ToInt32(str_samples);
                int sqrt_samples = (int)Math.Floor(Math.Sqrt(int_samples));
                if (sqrt_samples * sqrt_samples == int_samples)
                {
                    _world.CurrentViewPlane.NumSamples = int_samples;
                    
                }
                else
                {
                    Console.WriteLine("Given number of samples (" + int_samples + ") is a not a square number, multisampling disabled.");
                }
            }
            GlobalVars.NUM_SAMPLES = _world.CurrentViewPlane.NumSamples;

            string str_sampler = scene.GetAttribute("sampler");
            if (!str_sampler.Equals(""))
            {
                _world.CurrentViewPlane.ViewPlaneSampler = Sampler.LoadSampler(str_sampler);
            }
            GlobalVars.VIEWPLANE_SAMPLER = _world.CurrentViewPlane.ViewPlaneSampler;

            string str_algorithm = scene.GetAttribute("algorithm");
            if (!str_algorithm.Equals(""))
            {
                //Determine raytracing algorithm
                if (str_algorithm.Equals("raycast"))
                {
                    _world.CurrentTracer = new RayCaster(_world);
                }
                else if (str_algorithm.Equals("whitted"))
                {
                    _world.CurrentTracer = new Whitted(_world);
                }
                else
                {
                    Console.WriteLine("Unknown algorithm: " + str_algorithm);
                }
            }

            string str_cam = scene.GetAttribute("camera");
            if (!str_cam.Equals(""))
            {
                XmlNode node_cam = scene.SelectSingleNode("camera[@id=\"" + str_cam + "\"]");
                if (node_cam != null)
                {
                    _world.Camera = Camera.LoadCamera((XmlElement)node_cam);
                }
                else
                {
                    Console.WriteLine("No camera is defined with id=" + str_cam);
                }
            }
            else
            {
                Console.WriteLine("No camera defined.");
            }

            //Cleanup
            _world.Camera.compute_uvw();
        }
        private void LoadLights(XmlElement scene)
        {
            //First obtain reference to ambient light as defined in the body of the scene
            string str_amb = scene.GetAttribute("amblight");
            if (!str_amb.Equals(""))
            {
                XmlNode node_amb = scene.SelectSingleNode("light[@id=\"" + str_amb + "\" and @type=\"ambient\"]");
                if (node_amb != null)
                {
                    _world.AmbientLight = AmbientLight.LoadAmbient((XmlElement)node_amb);
                }
                else
                {
                    Console.WriteLine("No ambient light with id " + str_amb + " found.");
                }
            }

            //Get all references to other types of lights in the scene
            XmlNodeList light_list = scene.SelectNodes("light[@type!=\"ambient\"]");

            foreach (XmlNode light in light_list)
            {
                Light toAdd = Light.LoadLight((XmlElement)light);
                _world.AddLight(toAdd);
            }
        }
    }
}