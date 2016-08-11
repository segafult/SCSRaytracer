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
    /// Infinitely small light with a point of origin, color and intensity. Subject to no distance attenuation.
    /// </summary>
    class PointLight : Light
    {
        private float intensity;
        private Point3D location;

        //Constructors
        public PointLight()
        {
            color = new RGBColor(1, 1, 1);
            intensity = 0.5f;
            location = new Point3D(0, 0, 0);
            shadows = false;
        }
        public PointLight(Point3D l)
        {
            color = new RGBColor(1, 1, 1);
            intensity = 0.5f;
            location = new Point3D(l);
            shadows = false;
        }
        public PointLight(RGBColor c, float i, Point3D l)
        {
            color = new RGBColor(c);
            intensity = i;
            location = new Point3D(l);
            shadows = false;
        }

        //Gets and sets
        
        public float getIntensity() { return intensity; }
        public override Vect3D getDirection(ShadeRec sr) { return ((location - sr.HitPoint).Hat()); }
        public override bool castsShadows(){ return shadows; }
        public void setIntensity(float i) { intensity = i; }
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
            int num_objects = sr.WorldPointer.RenderList.Count;
            float tmin;

            //Find the closest intersection point along the given ray
            for (int i = 0; i < num_objects; i++)
            {
                tmin = (location - sr.HitPoint).Coordinates.Length();
                if (sr.WorldPointer.RenderList[i].Hit(ray, tmin))
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
                //if (point != 0)
                //{
                    toReturn.setLocation(point);
                //}
            }
            XmlNode node_int = lightRoot.SelectSingleNode("intensity");
            if (node_int != null)
            {
                string str_int = ((XmlText)node_int.FirstChild).Data;
                float intensity = Convert.ToSingle(str_int);
                toReturn.setIntensity(intensity);
            }

            return toReturn;
        }
    }
}
