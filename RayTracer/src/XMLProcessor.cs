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
using System.Xml;

namespace RayTracer
{
    public class XMLProcessor
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
                    this.LoadPlanes(objRoot, 0);
                    this.LoadSpheres(objRoot, 0);
                    this.LoadToruses(objRoot, 0);
                    this.LoadTrianglePrimitives(objRoot, 0);
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
                this.LoadPlanes(scene, 1);
                this.LoadSpheres(scene, 1);
                this.LoadToruses(scene, 1);
                this.LoadTrianglePrimitives(scene, 1);
                this.LoadInstances(scene);
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
                            double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                            matte.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
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
                            matte.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
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
                            double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                            phong.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
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
                            phong.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                            phong.setKs(ksDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
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
                            double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                            reflective.setKa(kaDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
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
                            reflective.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch (System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                            reflective.setKs(ksDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
                            reflective.setExp(expDouble);
                        }
                    }
                    catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kr if provided
                        XmlNode kr = definition.SelectSingleNode("kr");
                        if (kr != null)
                        {
                            double krDouble = Convert.ToDouble(((XmlText)kr.FirstChild).Data);
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


        //-------------------------------------------------------------------------------------------------------
        //   OBJECT LOADERS
        //-------------------------------------------------------------------------------------------------------
        private void LoadPlanes(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("plane");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    Plane plane = LoadPlane((XmlElement)definition);

                    switch (flag)
                    {
                        case 0:
                            w.objectList.Add(plane);
                            break;
                        case 1:
                            w.add_Object(plane);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Plane definition lacks an id handle and will be skipped.");
                }
            }
        }
        private Plane LoadPlane(XmlElement def)
        {
            Plane toReturn = new Plane();
            toReturn.id = def.GetAttribute("id");
            toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));

            //Load point if provided
            XmlNode p = def.SelectSingleNode("point");
            if (p != null)
            {
                string pText = ((XmlText)p.FirstChild).Data;
                Point3D pObj = Point3D.FromCsv(pText);
                if (pObj != null)
                {
                    toReturn.setP(pObj);
                }
            }

            //Load normal if provided
            XmlNode n = def.SelectSingleNode("normal");
            if (n != null)
            {
                string nText = ((XmlText)n.FirstChild).Data;
                Normal nObj = Normal.FromCsv(nText);
                if (nObj != null)
                {
                    toReturn.setN(nObj);
                }
            }

            return toReturn;
        }
        private void LoadSpheres(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("sphere");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    Sphere sphere = LoadSphere((XmlElement)definition);

                    switch (flag)
                    {
                        case 0:
                            w.objectList.Add(sphere);
                            break;
                        case 1:
                            w.add_Object(sphere);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Sphere definition lacks an id handle and will be skipped.");
                }
            }
        }
        private Sphere LoadSphere(XmlElement def)
        {
            Sphere toReturn = new Sphere();
            toReturn.id = def.GetAttribute("id");
            toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));

            //Load center of the sphere if provided
            XmlNode c = def.SelectSingleNode("point");
            if (c != null)
            {
                string cText = ((XmlText)c.FirstChild).Data;
                Point3D cObj = Point3D.FromCsv(cText);
                if (cObj != null)
                {
                    toReturn.set_center(cObj);
                }
            }

            try {
                //Load radius of the sphere if provided
                XmlNode r = def.SelectSingleNode("r");
                if (r != null)
                {
                    double rDouble = Convert.ToDouble(((XmlText)r.FirstChild).Data);
                    toReturn.set_radius(rDouble);
                }
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e.ToString());
            }

            return toReturn;
        }
        private void LoadToruses(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("torus");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    Torus torus = LoadTorus((XmlElement)definition);

                    switch (flag)
                    {
                        case 0:
                            w.objectList.Add(torus);
                            break;
                        case 1:
                            w.add_Object(torus);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Torus definition lacks an id handle and will be skipped.");
                }
            }
        }
        private Torus LoadTorus(XmlElement def)
        {
            Torus toReturn = new Torus();
            toReturn.id = def.GetAttribute("id");
            toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));

            //Load a if provided
            try
            {
                XmlNode a = def.SelectSingleNode("a");
                if (a != null)
                {
                    double aDouble = Convert.ToDouble(((XmlText)a.FirstChild).Data);
                    toReturn.setA(aDouble);
                }
            }
            catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

            //Load b if provided
            try
            {
                XmlNode b = def.SelectSingleNode("b");
                if (b != null)
                {
                    double bDouble = Convert.ToDouble(((XmlText)b.FirstChild).Data);
                    toReturn.setB(bDouble);

                }
            }
            catch (System.FormatException e) { Console.WriteLine(e.ToString()); }
            return toReturn;
        }
        private void LoadTrianglePrimitives(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("triangle");
            foreach (XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    Triangle triangle = LoadTrianglePrimitive((XmlElement)definition);

                    switch (flag)
                    {
                        case 0:
                            w.objectList.Add(triangle);
                            break;
                        case 1:
                            w.add_Object(triangle);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Triangle definition lacks an id handle and will be skipped.");
                }
            }
        }
        public Triangle LoadTrianglePrimitive(XmlElement def)
        {
            Triangle toReturn = new Triangle();
            toReturn.id = def.GetAttribute("id");
            toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));

            //Check if a list of vertices have been defined
            XmlNode vertRoot = def.SelectSingleNode("vertices");
            if (vertRoot != null)
            {
                //Get a list of all defined points
                XmlNodeList vertList = vertRoot.SelectNodes("point");
                //Need to be 3 vertices defined
                if (vertList.Count == 3)
                {
                    List<Point3D> verts_objs = new List<Point3D>(3);
                    foreach (XmlNode element in vertList)
                    {
                        verts_objs.Add(Point3D.FromCsv(((XmlText)element.FirstChild).Data));
                    }
                    toReturn.setVertices(verts_objs[0], verts_objs[1], verts_objs[2]);
                }
                else
                {
                    Console.WriteLine("Error: Exactly 3 vertices must be defined for a triangle primitive if <vertices> tags present.");
                }
            }

            return toReturn;
        }
        //-------------------------------------------------------------------------------------------------------
        //   END OBJECT LOADERS
        //-------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------
        //   WORLDSPACE AND INSTANCING LOADERS
        //-------------------------------------------------------------------------------------------------------
        private void LoadInstances(XmlNode objRoot)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("instance");

            foreach (XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if (((XmlElement)definition).HasAttribute("id"))
                {
                    Instance instance = LoadInstance((XmlElement)definition);

                    w.add_Object(instance);
                }
                else
                {
                    Console.WriteLine("Warning: Instance definition lacks an id handle and will be skipped.");
                }
            }
        }
        private Instance LoadInstance(XmlElement def)
        {
            Instance toReturn = new Instance();
            toReturn.id = def.GetAttribute("id");
            try {
                if (def.HasAttribute("mat"))
                {
                    toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));
                }

                if (def.HasAttribute("obj"))
                {
                    //Verify that object definition has been previously defined.
                    RenderableObject objRef = w.getObjectById(def.GetAttribute("obj"));
                    if (objRef != null)
                    {
                        toReturn.setHandle(objRef);
                    }
                    else
                    {
                        throw new XmlException("Error: No object definition with handle: " + def.GetAttribute("obj"));
                    }

                    Matrix baseTransformation = new Matrix();

                    //If defined, get rotation data
                    XmlNode rotNode = def.SelectSingleNode("rotate");
                    if (rotNode != null)
                    {
                        string rotString = ((XmlText)rotNode.FirstChild).Data;
                        Vect3D rotations = Vect3D.FromCsv(rotString);
                        //Rotation format valid?
                        if (rotations != null)
                        {
                            //Accumulate rotation on base transformation matrix
                            baseTransformation = baseTransformation * Matrix.rotateDeg(rotations);
                        }
                    }

                    //If defined, get scaling data
                    XmlNode scaleNode = def.SelectSingleNode("scale");
                    if (scaleNode != null)
                    {
                        string scaleString = ((XmlText)scaleNode.FirstChild).Data;
                        Vect3D scaling = Vect3D.FromCsv(scaleString);
                        //Scaling format valid?
                        if (scaling != null)
                        {
                            //Accumulate scaling on base transformation matrix
                            baseTransformation = baseTransformation * Matrix.scale(scaling);
                        }
                    }

                    //If defined, get translation data
                    XmlNode transNode = def.SelectSingleNode("translate");
                    if (transNode != null)
                    {
                        string transString = ((XmlText)transNode.FirstChild).Data;
                        Vect3D translation = Vect3D.FromCsv(transString);
                        //Translation format valid?
                        if (translation != null)
                        {
                            //Accumulate translation on base transformation matrix
                            baseTransformation = baseTransformation * Matrix.translate(translation);
                        }
                    }

                    //Apply accumulated transformations to instance
                    toReturn.applyTransformation(baseTransformation);
                }
                else
                {
                    throw new XmlException("Error: Cannot create instance without obj paired with an object id.");
                }
            }
            catch (XmlException e) { Console.WriteLine(e.ToString()); }

            return toReturn;
        }
        private void SetupWorldParameters(XmlElement scene)
        {
            //Setup world according to default parameters before doing anything.
            w.vp.set_hres(800);
            w.vp.set_vres(600);
            w.vp.set_gamma(1.0);
            w.vp.set_pixel_size(1.0);
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
                w.vp.set_gamma(Convert.ToDouble(str_gamma));
            }
            string str_pixelsize = scene.GetAttribute("px");
            if (!str_pixelsize.Equals(""))
            {
                w.vp.set_pixel_size(Convert.ToDouble(str_pixelsize));
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

            string str_sampler = scene.GetAttribute("sampler");
            if (!str_sampler.Equals(""))
            {
                //Determine sampler type and load accordingly.
                if (str_sampler.Equals("regular"))
                {
                    w.vp.set_sampler(new RegularSampler(w.vp.numSamples));
                }
                else
                {
                    Console.WriteLine("Unknown sampler type: " + str_sampler);
                }
            }

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
                    w.set_camera(LoadCamera((XmlElement)node_cam));
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
            w.vp.vpSampler.generate_samples();
            w.camera.compute_uvw();
        }
        private Camera LoadCamera(XmlElement camRoot)
        {
            string cam_type = camRoot.GetAttribute("type");
            Camera toReturn = new PinholeCamera();
            //If no camera type is defined, then return a default pinhole camera.
            if (!cam_type.Equals(""))
            {
                //Load subtype specific parameters in their own methods
                if (cam_type.Equals("pinhole"))
                {
                    toReturn = LoadPinholeCamera(camRoot);
                }
                else
                {
                    Console.WriteLine("Unknown camera type: " + cam_type);
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

                return toReturn;
            }
            else
            {
                Console.WriteLine("Camera type for camera " + camRoot.GetAttribute("id") + " not defined.");
                return toReturn;
            }
        }
        private PinholeCamera LoadPinholeCamera(XmlElement camRoot)
        {
            PinholeCamera toReturn = new PinholeCamera();

            XmlNode node_zoom = camRoot.SelectSingleNode("zoom");
            if (node_zoom != null)
            {
                string str_zoom = ((XmlText)node_zoom.FirstChild).Data;
                double zoom = Convert.ToDouble(str_zoom);
                toReturn.setZoom(zoom);
            }
            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                double vdp = Convert.ToDouble(str_vdp);
                toReturn.setVdp(vdp);
            }
            return toReturn;
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
                    w.set_ambient_light(LoadAmbient((XmlElement)node_amb));
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
                XmlElement light_element = (XmlElement)light;
                string light_type = light_element.GetAttribute("type");
                Light toAdd = new PointLight();
                if (light_type.Equals("point"))
                {
                    toAdd = LoadPointLight(light_element);
                }
                else if (light_type.Equals("directional"))
                {
                    toAdd = LoadDirectionalLight(light_element);
                }
                else
                {
                    Console.WriteLine("Unknown light type " + light_type + ", treating as point light");
                }

                //Load attributes common to all lights
                string node_shadow = light_element.GetAttribute("shadow");
                if (!node_shadow.Equals(""))
                {
                    toAdd.setShadow(Convert.ToBoolean(node_shadow));
                }
                XmlNode node_color = light_element.SelectSingleNode("color");
                if (node_color != null)
                {
                    string str_color = ((XmlText)node_color.FirstChild).Data;
                    RGBColor color = new RGBColor(System.Drawing.ColorTranslator.FromHtml(str_color));
                    if (color != null)
                    {
                        toAdd.setColor(color);
                    }
                }

                w.add_Light(toAdd);
            }
        }
        private AmbientLight LoadAmbient(XmlElement lightRoot)
        {
            AmbientLight toReturn = new AmbientLight();

            XmlNode node_intensity = lightRoot.SelectSingleNode("intensity");
            if (node_intensity != null)
            {
                string str_intensity = ((XmlText)node_intensity.FirstChild).Data;
                double intensity = Convert.ToDouble(str_intensity);
                toReturn.setIntensity(intensity);
            }

            XmlNode node_color = lightRoot.SelectSingleNode("color");
            if (node_color != null)
            {
                string str_color = ((XmlText)node_color.FirstChild).Data;
                RGBColor color = new RGBColor(System.Drawing.ColorTranslator.FromHtml(str_color));
                toReturn.setColor(color);
            }
            return toReturn;
        }
        private PointLight LoadPointLight(XmlElement lightRoot)
        {
            PointLight toReturn = new PointLight();

            //Load all provided attributes unique to point lights
            XmlNode node_point = lightRoot.SelectSingleNode("point");
            if (node_point != null)
            {
                string str_point = ((XmlText)node_point.FirstChild).Data;
                Point3D point = Point3D.FromCsv(str_point);
                if (point != null)
                {
                    toReturn.setLocation(point);
                }
            }
            XmlNode node_int = lightRoot.SelectSingleNode("intensity");
            if (node_int != null)
            {
                string str_int = ((XmlText)node_int.FirstChild).Data;
                double intensity = Convert.ToDouble(str_int);
                toReturn.setIntensity(intensity);
            }

            return toReturn;
        }
        private DirectionalLight LoadDirectionalLight(XmlElement lightRoot)
        {
            DirectionalLight toReturn = new DirectionalLight();

            //Load all attributes unique to directional lights
            XmlNode node_dir = lightRoot.SelectSingleNode("vector");
            if(node_dir != null)
            {
                string str_dir = ((XmlText)node_dir.FirstChild).Data;
                Vect3D direction = Vect3D.FromCsv(str_dir);
                if(direction != null)
                {
                    toReturn.setDirection(direction);
                }
            }
            XmlNode node_int = lightRoot.SelectSingleNode("intensity");
            if (node_int != null)
            {
                string str_int = ((XmlText)node_int.FirstChild).Data;
                double intensity = Convert.ToDouble(str_int);
                toReturn.setIntensity(intensity);
            }

            return toReturn;
        }
    }
}
