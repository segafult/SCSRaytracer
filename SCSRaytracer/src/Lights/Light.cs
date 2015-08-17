//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;

namespace RayTracer
{
    /// <summary>
    /// Template class, Light.
    /// </summary>
    abstract class Light
    {
        protected bool shadows;
        protected RGBColor color;

        abstract public RGBColor L(ShadeRec sr);
        public virtual bool inShadow(ShadeRec sr, Ray ray) { return false; }
        public virtual Vect3D getDirection(ShadeRec sr) { return new Vect3D(0, 0, 0); }
        public virtual bool castsShadows() { return shadows; }
        public virtual void setShadow(bool shad) { shadows = shad; }
        public RGBColor getColor() { return color; }
        public void setColor(RGBColor c) { color = new RGBColor(c); }

        public static Light LoadLight(XmlElement lightRoot)
        {
            //Determine light type...
            Light toReturn;
            string light_type = lightRoot.GetAttribute("type");
            //...and defer loading task to correct loader.
            if (light_type.Equals("point"))
            {
                toReturn = PointLight.LoadPointLight(lightRoot);
            }
            else if (light_type.Equals("directional"))
            {
                toReturn = DirectionalLight.LoadDirectionalLight(lightRoot);
            }
            else
            {
                toReturn = new PointLight();
                Console.WriteLine("Unknown light type " + light_type + ", treating as point light");
            }

            //Load attributes common to all lights
            string node_shadow = lightRoot.GetAttribute("shadow");
            if (!node_shadow.Equals(""))
            {
                toReturn.setShadow(Convert.ToBoolean(node_shadow));
            }
            XmlNode node_color = lightRoot.SelectSingleNode("color");
            if (node_color != null)
            {
                string str_color = ((XmlText)node_color.FirstChild).Data;
                RGBColor color = new RGBColor(System.Drawing.ColorTranslator.FromHtml(str_color));
                //if (color.r != null)
                //{
                    toReturn.setColor(color);
                //}
            }
            return toReturn;
        }
    }
}
