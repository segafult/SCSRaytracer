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
            sceneXML = new XmlDocument();
            sceneReader = new XmlTextReader(filename);
            sceneXML.Load(sceneReader);
            root = sceneXML.DocumentElement;
            w = worldref;
        }

        public void LoadMaterials()
        {
            try {
                XmlNodeList mats = root.SelectNodes("materials");
                if(mats==null)
                    throw new XmlException("Invalid SCSML: No material tags present in XML document.");
                foreach (XmlNode matRoot in mats)
                {
                    //Load materials
                    this.LoadMatte(matRoot);
                    this.LoadPhong(matRoot);
                    this.LoadReflective(matRoot);
                }
            }
            catch(XmlException e)
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
            catch(XmlException e)
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

                this.LoadPlanes(scene, 1);
                this.LoadSpheres(scene, 1);
                this.LoadToruses(scene, 1);
                this.LoadTrianglePrimitives(scene, 1);
                this.LoadInstances(scene);
            }
            catch(XmlException e)
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
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                            matte.setKd(kdDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            matte.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch(System.Exception e) { Console.WriteLine(e.ToString()); }

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
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                            phong.setKd(kdDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            phong.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch(System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                            phong.setKs(ksDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
                            phong.setExp(expDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

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
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Kd value if provided
                        XmlNode kd = definition.SelectSingleNode("kd");
                        if (kd != null)
                        {
                            double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                            reflective.setKd(kdDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load color if provided
                        XmlNode cd = definition.SelectSingleNode("cd");
                        if (cd != null)
                        {
                            string cdStr = ((XmlText)cd.FirstChild).Data;
                            reflective.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                        }
                    }
                    catch(System.Exception e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load Ks value if provided
                        XmlNode ks = definition.SelectSingleNode("ks");
                        if (ks != null)
                        {
                            double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                            reflective.setKs(ksDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

                    try {
                        //Load specular exponent if provided
                        XmlNode exp = definition.SelectSingleNode("exp");
                        if (exp != null)
                        {
                            double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
                            reflective.setExp(expDouble);
                        }
                    }
                    catch(System.FormatException e) { Console.WriteLine(e.ToString()); }

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
            foreach(XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if(((XmlElement)definition).HasAttribute("id"))
                {
                    Plane plane = LoadPlane((XmlElement)definition);

                    switch(flag)
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
            if(p!=null)
            {
                string pText = ((XmlText)p.FirstChild).Data;
                Point3D pObj = Point3D.FromCsv(pText);
                if(pObj != null)
                {
                    toReturn.setP(pObj);
                }
            }

            //Load normal if provided
            XmlNode n = def.SelectSingleNode("normal");
            if(n!=null)
            {
                string nText = ((XmlText)n.FirstChild).Data;
                Normal nObj = Normal.FromCsv(nText);
                if(nObj != null)
                {
                    toReturn.setN(nObj);
                }
            }

            return toReturn;
        }
        private void LoadSpheres(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("sphere");
            foreach(XmlNode definition in currentContext)
            {
                //Filter all objects without an id handle
                if(((XmlElement)definition).HasAttribute("id"))
                {
                    Sphere sphere = LoadSphere((XmlElement)definition);

                    switch(flag)
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
            if(c != null)
            {
                string cText = ((XmlText)c.FirstChild).Data;
                Point3D cObj = Point3D.FromCsv(cText);
                if(cObj != null)
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
            catch(System.FormatException e)
            {
                Console.WriteLine(e.ToString());
            }

            return toReturn;
        }
        private void LoadToruses(XmlNode objRoot, int flag)
        {
            XmlNodeList currentContext = objRoot.SelectNodes("torus");
            foreach(XmlNode definition in currentContext)
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
                if(a!=null)
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
                if(b!=null)
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
            if(vertRoot != null)
            {
                //Get a list of all defined points
                XmlNodeList vertList = vertRoot.SelectNodes("point");
                //Need to be 3 vertices defined
                if(vertList.Count == 3)
                {
                    List<Point3D> verts_objs = new List<Point3D>(3);
                    foreach(XmlNode element in vertList)
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

            foreach(XmlNode definition in currentContext)
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
                    RenderableObject objRef = w.getObjectById(def.GetAttribute("obj"));
                    if(objRef != null)
                    {
                        toReturn.setHandle(objRef);
                    }
                    else
                    {
                        throw new XmlException("Error: No object definition with handle: " + def.GetAttribute("obj"));
                    }
                }
                else
                {
                    throw new XmlException("Error: Cannot create instance without obj paired with an object id.");
                }
            }
            catch(XmlException e) { Console.WriteLine(e.ToString()); }

            return toReturn;
        }
    }
}
