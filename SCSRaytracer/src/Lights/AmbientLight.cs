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
    /// Ambient light for basic world illumination
    /// </summary>
    class AmbientLight : Light
    {
        private float intensity;
        

        //Default constructor
        public AmbientLight()
        {
            intensity = 0.5f;
            color = new RGBColor(1, 1, 1);
        }
        public AmbientLight(RGBColor c, float i)
        {
            color = new RGBColor(c);
            intensity = i;
        }

        //Gets and sets
        public void setIntensity(float i) { intensity = i; }
        public float getIntensity() { return intensity; }
        public override bool castsShadows()
        {
            return false;
        }

        public override RGBColor L(ShadeRec sr)
        {
            return (intensity * color);
        }

        public override Vect3D getDirection(ShadeRec sr)
        {
            //Ambient light has no direction
            return new Vect3D(0, 0, 0);
        }

        public static AmbientLight LoadAmbient(XmlElement lightRoot)
        {
            AmbientLight toReturn = new AmbientLight();

            XmlNode node_intensity = lightRoot.SelectSingleNode("intensity");
            if (node_intensity != null)
            {
                string str_intensity = ((XmlText)node_intensity.FirstChild).Data;
                float intensity = Convert.ToSingle(str_intensity);
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
    }
}
