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
        private Point3D p;
        private Normal n;

        public Plane()
        {
            p = new Point3D(0, 0, 0);
            n = new Normal(0, 1, 0);
        }
        public Plane(Point3D point, Normal normal)
        {
            p = point;
            n = normal;
        }

        public override string ToString()
        {
            return "Plane primitive\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.getMaterial().id + "\n" +
                "  P: " + p.ToString() + "\n" +
                "  N: " + n.ToString();
        }

        //Gets and sets
        public void setP(Point3D parg) { p = parg; }
        public void setN(Normal narg) { n = narg; }
        public Point3D getP() { return p; }
        public Normal getN() { return n; }

        /// <summary>
        /// Determines t value for intersection of plane and given ray, passes shading info back through sr;
        /// </summary>
        /// <param name="r">Ray to determine intersection</param>
        /// <param name="tmin">Passed by reference, minimum t value</param>
        /// <param name="sr">ShadeRec to store shading info in</param>
        /// <returns></returns>
        override public bool hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            float t = (p - r.Origin) * n / (r.Direction * n);

            //Intersection is in front of camera
            if(t > GlobalVars.K_EPSILON && t < tmin)
            {
                tmin = t;
                sr.normal = n;
                sr.hit_point_local = r.Origin + t * r.Direction;
                sr.obj_material = mat;
                return true;
            }
            //Intersection is behind camera
            else
            {
                return false;
            }
        }

        public override bool hit(Ray r, float tmin)
        {
            float t = (p - r.Origin) * n / (r.Direction * n);

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
                    toReturn.setP(pObj);
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
                    toReturn.setN(nObj);
                //}
            }

            return toReturn;
        }
    }
}
