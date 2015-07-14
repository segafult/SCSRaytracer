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
        public void setColor(RGBColor c) { color = new RGBColor(c); }
        public void setIntensity(double i) { intensity = i; }
        public void setLocation(Point3D p) { p = new Point3D(p); }

        public override RGBColor L(ShadeRec sr)
        {
            return intensity * color;
        }
    }
}
