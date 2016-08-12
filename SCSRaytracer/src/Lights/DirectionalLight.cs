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
    /// Directional light, has no point source nor distance attenuation.
    /// Similar to sunslight
    /// </summary>
    class DirectionalLight : Light
    {
        private float _intensity;
        private Vect3D _direction;

        public override Vect3D Direction
        {
            get
            {
                return -_direction;
            }
            set
            {
                _direction = value.Hat();
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
        public DirectionalLight()
        {
            _direction = (new Vect3D(-1, -1, -1)).Hat();
            _intensity = 0.5f;
            _color = new RGBColor(1, 1, 1);
            _castsShadows = false;
        }
        public DirectionalLight(Vect3D d)
        {
            _direction = d.Hat();
            _intensity = 0.5f;
            _color = new RGBColor(1, 1, 1);
            _castsShadows = false;
        }
        public DirectionalLight(RGBColor c, float i, Vect3D d)
        {
            _color = new RGBColor(c);
            _intensity = i;
            _direction = d.Hat();
            _castsShadows = false;
        }

        //Gets and sets
        //public void setDirection(Vect3D d) { _direction = d.Hat(); }
        //public void setIntensity(float i) { _intensity = i; }
        //public void setShadows(bool shad) { _castsShadows = shad; }
        //public Vect3D getDirection() { return _direction; }
        //public float getIntensity() { return _intensity; }
         

        public override RGBColor GetLighting(ShadeRec sr)
        {
            return _intensity * _color;
        }
        public override bool InShadow(ShadeRec sr, Ray ray)
        {
            ShadeRec tempSr = sr.WorldPointer.HitObjects(ray);
            return tempSr.HitAnObject;
        }
        
        public override Vect3D GetDirection(ShadeRec sr)
        {
            return -_direction;
        }
        

        public static DirectionalLight LoadDirectionalLight(XmlElement lightRoot)
        {
            DirectionalLight toReturn = new DirectionalLight();

            //Load all attributes unique to directional lights
            XmlNode node_dir = lightRoot.SelectSingleNode("vector");
            if (node_dir != null)
            {
                string str_dir = ((XmlText)node_dir.FirstChild).Data;
                Vect3D direction = Vect3D.FromCsv(str_dir);
                //if (direction != null)
                //{
                    toReturn.Direction = direction;
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
