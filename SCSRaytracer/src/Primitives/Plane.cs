//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Xml;

namespace SCSRaytracer
{

    sealed class Plane : RenderableObject
    {
        private Point3D _p;
        private Normal _n;

        // Accessors
        public Point3D PointOnPlane
        {
            get
            {
                return _p;
            }
            set
            {
                _p = value;
            }
        }
        public Normal Normal
        {
            get
            {
                return _n;
            }
            set
            {
                _n = value;
            }
        }

        public Plane()
        {
            _p = new Point3D(0, 0, 0);
            _n = new Normal(0, 1, 0);
        }
        public Plane(Point3D point, Normal normal)
        {
            _p = point;
            _n = normal;
        }

        public override string ToString()
        {
            return "Plane primitive\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.Material.id + "\n" +
                "  P: " + _p.ToString() + "\n" +
                "  N: " + _n.ToString();
        }

        //Gets and sets
        /*
        public void setP(Point3D parg) { _p = parg; }
        public void setN(Normal narg) { _n = narg; }
        public Point3D getP() { return _p; }
        public Normal getN() { return _n; }
        */

        /// <summary>
        /// Determines t value for intersection of plane and given ray, passes shading info back through sr;
        /// </summary>
        /// <param name="r">Ray to determine intersection</param>
        /// <param name="tmin">Passed by reference, minimum t value</param>
        /// <param name="sr">ShadeRec to store shading info in</param>
        /// <returns></returns>
        override public bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            float t = (_p - r.Origin) * _n / (r.Direction * _n);

            //Intersection is in front of camera
            if(t > GlobalVars.K_EPSILON && t < tmin)
            {
                tmin = t;
                sr.Normal = _n;
                sr.HitPointLocal = r.Origin + t * r.Direction;
                sr.ObjectMaterial = _material;
                return true;
            }
            //Intersection is behind camera
            else
            {
                return false;
            }
        }

        public override bool Hit(Ray r, float tmin)
        {
            float t = (_p - r.Origin) * _n / (r.Direction * _n);

            //Intersection is in front of camera
            if (t > GlobalVars.K_EPSILON && t < tmin)
            {
                return true;
            }
            //Intersection is behind camera
            return false;
        }

        public static Plane LoadPlane(XmlElement def)
        {
            Plane toReturn = new Plane();

            //Load point if provided
            XmlNode p = def.SelectSingleNode("point");
            if (p != null)
            {
                string pText = ((XmlText)p.FirstChild).Data;
                Point3D pObj = Point3D.FromCsv(pText);
                //if (pObj != null)
                //{
                    toReturn.PointOnPlane = pObj;
                //}
            }

            //Load normal if provided
            XmlNode n = def.SelectSingleNode("normal");
            if (n != null)
            {
                string nText = ((XmlText)n.FirstChild).Data;
                Normal nObj = Normal.FromCsv(nText);
                //if (nObj != null)
                //{
                    toReturn.Normal = nObj;
                //}
            }

            return toReturn;
        }
    }
}
