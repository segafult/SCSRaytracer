//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;

namespace RayTracer
{
    sealed class XMLProcessor
    {
        private XmlDocument sceneXML;
        private XmlReader sceneReader;
        private XmlNode root;

        World w;

        public XMLProcessor(World worldref)
        {
            sceneXML = new XmlDocument();
            w = worldref;
        }
        public XMLProcessor(XmlReader doc, World worldref)
        {
            sceneXML = new XmlDocument();
            sceneReader = doc;
            sceneXML.Load(sceneReader);
            w = worldref;
        }
        public XMLProcessor(string filename, World worldref)
        {
            try {
                sceneXML = new XmlDocument();
                sceneReader = new XmlTextReader(filename);
                sceneXML.Load(sceneReader);
                root = sceneXML.DocumentElement;
                w = worldref;
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(1);
            }
        }

        public void LoadMaterials()
        {
            try {
                XmlNodeList mats = root.SelectNodes("materials");
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
        public void LoadObjects()
        {
            try
            {
                XmlNodeList objs = root.SelectNodes("objects");
                if (objs == null)
                    throw new XmlException("Invalid SCSML: No object tags present in XML document.");
                foreach (XmlNode objRoot in objs) {
                    //Get the list of renderables
                    XmlNodeList renderables = objRoot.SelectNodes("renderable");

                    foreach(XmlElement rendRoot in renderables)
                    {
                        RenderableObject rend = RenderableObject.LoadRenderableObject(rendRoot);
                        if (rend != null)
                            w.objectList.Add(rend);
                    }
                    /*this.LoadPlanes(objRoot, 0);
                    this.LoadSpheres(objRoot, 0);
                    this.LoadToruses(objRoot, 0);
                    this.LoadTrianglePrimitives(objRoot, 0);
                    this.LoadBoxes(objRoot, 0);
                    */
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
                XmlNode scene = root.SelectSingleNode("scene");
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
                        w.add_Object(myrend);
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
                            matte.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            matte.setKd(kdDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;

							//matte.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));

                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    w.materialList.Add(matte);
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
                            phong.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            phong.setKd(kdDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            //phong.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            float ksDouble = Convert.ToSingle(((XmlText)ks.FirstChild).Data);
                            phong.setKs(ksDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            float expDouble = Convert.ToSingle(((XmlText)exp.FirstChild).Data);
                            phong.setExp(expDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    w.materialList.Add(phong);
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
                            reflective.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            float kdDouble = Convert.ToSingle(((XmlText)kd.FirstChild).Data);
                            reflective.setKd(kdDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            //reflective.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            float ksDouble = Convert.ToSingle(((XmlText)ks.FirstChild).Data);
                            reflective.setKs(ksDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            float expDouble = Convert.ToSingle(((XmlText)exp.FirstChild).Data);
                            reflective.setExp(expDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kr if provided
                        XmlNode kr = definition.SelectSingleNode("kr");
                        if (kr != null)
                        {
                            float krDouble = Convert.ToSingle(((XmlText)kr.FirstChild).Data);
                            reflective.setReflectivity(krDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }


                    w.materialList.Add(reflective);
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
            w.vp.set_hres(GlobalVars.hres);
            w.vp.set_vres(GlobalVars.vres);
            w.vp.set_gamma(1.0f);
            w.vp.set_pixel_size(1.0f);
            w.vp.set_max_depth(5);
            w.vp.set_samples(1);
            w.vp.set_sampler(new RegularSampler(w.vp.numSamples));
            w.vp.vpSampler.generate_samples();
            w.set_camera(new PinholeCamera());
            w.tracer = new RayCaster(w);

            //Good to go, begin reading in parameters as provided
            string str_hres = scene.GetAttribute("hres");
            if (!str_hres.Equals(""))
            {
                w.vp.set_hres(Convert.ToInt32(str_hres));
            }
            string str_vres = scene.GetAttribute("vres");
            if (!str_vres.Equals(""))
            {
                w.vp.set_vres(Convert.ToInt32(str_vres));
            }
            string str_gamma = scene.GetAttribute("gamma");
            if (!str_gamma.Equals(""))
            {
                w.vp.set_gamma(Convert.ToSingle(str_gamma));
            }
            string str_pixelsize = scene.GetAttribute("px");
            if (!str_pixelsize.Equals(""))
            {
                w.vp.set_pixel_size(Convert.ToSingle(str_pixelsize));
            }
            string str_renderdepth = scene.GetAttribute("renderdepth");
            if (!str_pixelsize.Equals(""))
            {
                w.vp.set_max_depth(Convert.ToInt32(str_renderdepth));
            }
            string str_samples = scene.GetAttribute("multisample");
            if (!str_samples.Equals(""))
            {
                //Check if the number of samples is a perfectly square number
                int int_samples = Convert.ToInt32(str_samples);
                int sqrt_samples = (int)Math.Floor(Math.Sqrt(int_samples));
                if (sqrt_samples * sqrt_samples == int_samples)
                {
                    w.vp.set_numSamples(int_samples);
                    
                }
                else
                {
                    Console.WriteLine("Given number of samples (" + int_samples + ") is a not a square number, multisampling disabled.");
                }
            }
            GlobalVars.num_samples = w.vp.numSamples;

            string str_sampler = scene.GetAttribute("sampler");
            if (!str_sampler.Equals(""))
            {
                w.vp.set_sampler(Sampler.LoadSampler(str_sampler));
            }
            GlobalVars.vp_sampler = w.vp.vpSampler;

            string str_algorithm = scene.GetAttribute("algorithm");
            if (!str_algorithm.Equals(""))
            {
                //Determine raytracing algorithm
                if (str_algorithm.Equals("raycast"))
                {
                    w.tracer = new RayCaster(w);
                }
                else if (str_algorithm.Equals("whitted"))
                {
                    w.tracer = new Whitted(w);
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
                    w.set_camera(Camera.LoadCamera((XmlElement)node_cam));
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
            w.camera.compute_uvw();
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
                    w.set_ambient_light(AmbientLight.LoadAmbient((XmlElement)node_amb));
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
                w.add_Light(toAdd);
            }
        }
    }
}