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
using System.Xml;

namespace RayTracer
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
