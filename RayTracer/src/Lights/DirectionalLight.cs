using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Directional light, has no point source nor distance attenuation.
    /// Similar to sunslight
    /// </summary>
    public class DirectionalLight : Light
    {
        private double intensity;
        private Vect3D direction;
        private RGBColor color;

        //Constructors
        public DirectionalLight(Vect3D d)
        {
            direction = d.hat();
            intensity = 0.5;
            color = new RGBColor(1, 1, 1);
        }
        public DirectionalLight(RGBColor c, double i, Vect3D d)
        {
            color = new RGBColor(c);
            intensity = i;
            direction = d.hat();
        }

        //Gets and sets
        public void setColor(RGBColor c) { color = new RGBColor(c); }
        public void setDirection(Vect3D d) { direction = d.hat(); }
        public void setIntensity(double i) { intensity = i; }
        public RGBColor getColor() { return color; }
        public Vect3D getDirection() { return direction; }
        public double getIntensity() { return intensity; } 

        public override RGBColor L(ShadeRec sr)
        {
            return intensity * color;
        }
    }
}
