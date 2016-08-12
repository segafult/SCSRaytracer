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
        private float _intensity;

        public float Intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                _intensity = value;
            }
        }
        public override bool CastsShadows
        {
            get
            {
                return false;
            }
        }

        //Default constructor
        public AmbientLight()
        {
            _intensity = 0.5f;
            _color = new RGBColor(1, 1, 1);
        }
        public AmbientLight(RGBColor c, float i)
        {
            _color = new RGBColor(c);
            _intensity = i;
        }

        //Gets and sets
        public void setIntensity(float i) { _intensity = i; }
        public float getIntensity() { return _intensity; }
        /*
        public override bool castsShadows()
        {
            return false;
        }
        */

        public override RGBColor GetLighting(ShadeRec sr)
        {
            return (_intensity * _color);
        }

        /*
        public override Vect3D getDirection(ShadeRec sr)
        {
            //Ambient light has no direction
            return new Vect3D(0, 0, 0);
        }
        */

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
                toReturn.Color = color;
            }
            return toReturn;
        }
    }
}
