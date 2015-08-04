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

        //Constructors
        public DirectionalLight()
        {
            direction = (new Vect3D(-1, -1, -1)).hat();
            intensity = 0.5;
            color = new RGBColor(1, 1, 1);
            shadows = false;
        }
        public DirectionalLight(Vect3D d)
        {
            direction = d.hat();
            intensity = 0.5;
            color = new RGBColor(1, 1, 1);
            shadows = false;
        }
        public DirectionalLight(RGBColor c, double i, Vect3D d)
        {
            color = new RGBColor(c);
            intensity = i;
            direction = d.hat();
            shadows = false;
        }

        //Gets and sets
        public void setDirection(Vect3D d) { direction = d.hat(); }
        public void setIntensity(double i) { intensity = i; }
        public void setShadows(bool shad) { shadows = shad; }
        public Vect3D getDirection() { return direction; }
        public double getIntensity() { return intensity; }
         

        public override RGBColor L(ShadeRec sr)
        {
            return intensity * color;
        }
        public override bool inShadow(ShadeRec sr, Ray ray)
        {
            ShadeRec tempSr = sr.w.hit_objects(ray);
            return tempSr.hit_an_object;
        }
    }
}
