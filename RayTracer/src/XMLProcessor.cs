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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    this.LoadPlanes(objRoot,0);
                }
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

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

                    //Load Ka value if provided
                    XmlNode ka = definition.SelectSingleNode("ka");
                    if (ka!=null)
                    {
                        double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                        matte.setKa(kaDouble);
                    }
                    //Load Kd value if provided
                    XmlNode kd = definition.SelectSingleNode("kd");
                    if(kd!=null)
                    {
                        double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                        matte.setKd(kdDouble);
                    }
                    //Load color if provided
                    XmlNode cd = definition.SelectSingleNode("cd");
                    if (cd != null)
                    {
                        string cdStr = ((XmlText)cd.FirstChild).Data;
                        matte.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                    }
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

                    //Load Ka value if provided
                    XmlNode ka = definition.SelectSingleNode("ka");
                    if (ka != null)
                    {
                        double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                        phong.setKa(kaDouble);
                    }
                    //Load Kd value if provided
                    XmlNode kd = definition.SelectSingleNode("kd");
                    if (kd != null)
                    {
                        double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                        phong.setKd(kdDouble);
                    }
                    //Load color if provided
                    XmlNode cd = definition.SelectSingleNode("cd");
                    if (cd != null)
                    {
                        string cdStr = ((XmlText)cd.FirstChild).Data;
                        phong.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                    }
                    //Load Ks value if provided
                    XmlNode ks = definition.SelectSingleNode("ks");
                    if(ks != null)
                    {
                        double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                        phong.setKs(ksDouble);
                    }
                    //Load specular exponent if provided
                    XmlNode exp = definition.SelectSingleNode("exp");
                    if(exp!=null)
                    {
                        double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
                        phong.setExp(expDouble);
                    }
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

                    //Load Ka value if provided
                    XmlNode ka = definition.SelectSingleNode("ka");
                    if (ka != null)
                    {
                        double kaDouble = Convert.ToDouble(((XmlText)ka.FirstChild).Data);
                        reflective.setKa(kaDouble);
                    }
                    //Load Kd value if provided
                    XmlNode kd = definition.SelectSingleNode("kd");
                    if (kd != null)
                    {
                        double kdDouble = Convert.ToDouble(((XmlText)kd.FirstChild).Data);
                        reflective.setKd(kdDouble);
                    }
                    //Load color if provided
                    XmlNode cd = definition.SelectSingleNode("cd");
                    if (cd != null)
                    {
                        string cdStr = ((XmlText)cd.FirstChild).Data;
                        reflective.setCd(new RGBColor(System.Drawing.ColorTranslator.FromHtml(cdStr)));
                    }
                    //Load Ks value if provided
                    XmlNode ks = definition.SelectSingleNode("ks");
                    if (ks != null)
                    {
                        double ksDouble = Convert.ToDouble(((XmlText)ks.FirstChild).Data);
                        reflective.setKs(ksDouble);
                    }
                    //Load specular exponent if provided
                    XmlNode exp = definition.SelectSingleNode("exp");
                    if (exp != null)
                    {
                        double expDouble = Convert.ToDouble(((XmlText)exp.FirstChild).Data);
                        reflective.setExp(expDouble);
                    }
                    //Load Kr if provided
                    XmlNode kr = definition.SelectSingleNode("kr");
                    if(kr != null)
                    {
                        double krDouble = Convert.ToDouble(((XmlText)kr.FirstChild).Data);
                        reflective.setReflectivity(krDouble);
                    }
                    w.materialList.Add(reflective);
                }
                else
                {
                    Console.WriteLine("Warning: Reflective shader definition lacks an id handle and will be skipped.");
                }
            }
        }

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
                            w.renderList.Add(plane);
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
            toReturn.setMaterial(Material.getMaterialById(w,def.GetAttribute("mat")));

            return toReturn;
        }
    }
}
