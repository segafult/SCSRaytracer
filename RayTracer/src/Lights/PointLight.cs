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
    /// Infinitely small light with a point of origin, color and intensity. Subject to distance attenuation.
    /// </summary>
    public class PointLight : Light
    {
        private double intensity;
        private Point3D location;

        //Constructors
        public PointLight()
        {
            color = new RGBColor(1, 1, 1);
            intensity = 0.5;
            location = new Point3D(0, 0, 0);
            shadows = false;
        }
        public PointLight(Point3D l)
        {
            color = new RGBColor(1, 1, 1);
            intensity = 0.5;
            location = new Point3D(l);
            shadows = false;
        }
        public PointLight(RGBColor c, double i, Point3D l)
        {
            color = new RGBColor(c);
            intensity = i;
            location = new Point3D(l);
            shadows = false;
        }

        //Gets and sets
        
        public double getIntensity() { return intensity; }
        public override Vect3D getDirection(ShadeRec sr) { return ((location - sr.hit_point).hat()); }
        public override bool castsShadows(){ return shadows; }
        public void setIntensity(double i) { intensity = i; }
        public void setLocation(Point3D p) { location = new Point3D(p); }

        public override RGBColor L(ShadeRec sr)
        {
            return intensity * color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr">Shading parameters</param>
        /// <param name="ray">Ray to cast from object to point light</param>
        /// <returns></returns>
        public override bool inShadow(ShadeRec sr, Ray ray)
        {
            int num_objects = sr.w.renderList.Count;
            double tmin;

            //Find the closest intersection point along the given ray
            for (int i = 0; i < num_objects; i++)
            {
                tmin = (location - sr.hit_point).magnitude();
                if (sr.w.renderList[i].hit(ray, tmin))
                {
                    return true;
                }
            }
            return false;
        }

        public static PointLight LoadPointLight(XmlElement lightRoot)
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
    }
}
