//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;

namespace SCSRaytracer
{
    /// <summary>
    /// Template class, Light.
    /// </summary>
    abstract class Light
    {
        protected bool _castsShadows;
        protected RGBColor _color;

        public virtual bool CastsShadows
        {
            get
            {
                return _castsShadows;
            }
            set
            {
                _castsShadows = value;
            }
        }
        public RGBColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = new RGBColor(value);
            }
        }
        public virtual Vect3D Direction
        {
            get
            {
                return new Vect3D(0, 0, 0);
            }
            set
            {
                // do nothing
            }
        }

        abstract public RGBColor GetLighting(ShadeRec sr);
        public virtual bool InShadow(ShadeRec sr, Ray ray) { return false; }
        public virtual Vect3D GetDirection(ShadeRec sr) { return new Vect3D(0, 0, 0); }
        //public virtual bool castsShadows() { return _castsShadows; }
        //public virtual void setShadow(bool shad) { _castsShadows = shad; }
        //public RGBColor getColor() { return _color; }
        //public void setColor(RGBColor c) { _color = new RGBColor(c); }

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
                toReturn.CastsShadows = Convert.ToBoolean(node_shadow);
            }
            XmlNode node_color = lightRoot.SelectSingleNode("color");
            if (node_color != null)
            {
                string str_color = ((XmlText)node_color.FirstChild).Data;
                RGBColor color = new RGBColor(System.Drawing.ColorTranslator.FromHtml(str_color));
                //if (color.r != null)
                //{
                    toReturn.Color = color;
                //}
            }
            return toReturn;
        }
    }
}
