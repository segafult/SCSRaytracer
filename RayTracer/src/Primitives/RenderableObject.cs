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
    public abstract class RenderableObject
    {
        public string id;
        //public RGBColor color;
        protected Material mat;

        public virtual bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            return false;
        }
        public virtual bool hit(Ray r, double tmin)
        {
            return false;
        }
        public virtual Material getMaterial() { return mat; }
        public virtual void setMaterial(Material m)
        {
            mat = m;
        }

        public virtual BoundingBox get_bounding_box()
        {
            return new BoundingBox();
        }

    }
}
