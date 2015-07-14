using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Ambient light for basic world illumination
    /// </summary>
    public class AmbientLight : Light
    {
        private double intensity;
        private RGBColor color;

        //Default constructor
        public AmbientLight()
        {
            intensity = 0.5;
            color = new RGBColor(1, 1, 1);
        }
        public AmbientLight(RGBColor c, double i)
        {
            color = new RGBColor(c);
            intensity = i;
        }

        //Gets and sets
        public void setColor(RGBColor c) { color = new RGBColor(c); }
        public void setIntensity(double i) { intensity = i; }
        public RGBColor getColor() { return color; }
        public double getIntensity() { return intensity; }

        public override RGBColor L(ShadeRec sr)
        {
            return (intensity * color);
        }

        public override Vect3D getDirection(ShadeRec sr)
        {
            //Ambient light has no direction
            return new Vect3D(0, 0, 0);
        }
    }
}
