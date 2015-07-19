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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Infinitely small light with a point of origin, color and intensity. Subject to distance attenuation.
    /// </summary>
    public class PointLight : Light
    {
        private RGBColor color;
        private double intensity;
        private Point3D location;

        //Constructors
        public PointLight(Point3D l)
        {
            color = new RGBColor(1, 1, 1);
            intensity = 0.5;
            location = new Point3D(l);
        }
        public PointLight(RGBColor c, double i, Point3D l)
        {
            color = new RGBColor(c);
            intensity = i;
            location = new Point3D(l);
        }

        //Gets and sets
        public RGBColor getColor() { return color; }
        public double getIntensity() { return intensity; }
        public override Vect3D getDirection(ShadeRec sr) { return ((location - sr.hit_point).hat()); }
        public override bool castsShadows(){ return shadows; }
        public void setColor(RGBColor c) { color = new RGBColor(c); }
        public void setIntensity(double i) { intensity = i; }
        public void setLocation(Point3D p) { p = new Point3D(p); }
        public void setShadow(bool shad) { shadows = shad; }

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

            //Find the closest intersection point along the given ray
            for (int i = 0; i < num_objects; i++)
            {
                if (sr.w.renderList[i].shadowHit(ray))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
