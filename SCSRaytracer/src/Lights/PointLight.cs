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
        private float _intensity;
        private Point3D _location;

        public override Vect3D Direction
        {
            get
            {
                return base.Direction;
            }

            set
            {
                base.Direction = value;
            }
        }
        public Point3D Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = new Point3D(value);
            }
        }
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

        //Constructors
        public PointLight()
        {
            _color = new RGBColor(1, 1, 1);
            _intensity = 0.5f;
            _location = new Point3D(0, 0, 0);
            _castsShadows = false;
        }
        public PointLight(Point3D l)
        {
            _color = new RGBColor(1, 1, 1);
            _intensity = 0.5f;
            _location = new Point3D(l);
            _castsShadows = false;
        }
        public PointLight(RGBColor c, float i, Point3D l)
        {
            _color = new RGBColor(c);
            _intensity = i;
            _location = new Point3D(l);
            _castsShadows = false;
        }

        //Gets and sets
        
        //public float getIntensity() { return _intensity; }
        public override Vect3D GetDirection(ShadeRec sr) { return ((_location - sr.HitPoint).Hat()); }
        //public override bool castsShadows(){ return _castsShadows; }
        //public void setIntensity(float i) { _intensity = i; }
        //public void setLocation(Point3D p) { _location = new Point3D(p); }

        public override RGBColor GetLighting(ShadeRec sr)
        {
            return _intensity * _color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr">Shading parameters</param>
        /// <param name="ray">Ray to cast from object to point light</param>
        /// <returns></returns>
        public override bool InShadow(ShadeRec sr, Ray ray)
        {
            int num_objects = sr.WorldPointer.RenderList.Count;
            float tmin;

            //Find the closest intersection point along the given ray
            for (int i = 0; i < num_objects; i++)
            {
                tmin = (_location - sr.HitPoint).Coordinates.Length();
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
                    toReturn.Location = point;
                //}
            }
            XmlNode node_int = lightRoot.SelectSingleNode("intensity");
            if (node_int != null)
            {
                string str_int = ((XmlText)node_int.FirstChild).Data;
                float intensity = Convert.ToSingle(str_int);
                toReturn.Intensity = intensity;
            }

            return toReturn;
        }
    }
}
