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
    /// Template class, Light.
    /// </summary>
    abstract public class Light
    {
        protected bool shadows;
        abstract public RGBColor L(ShadeRec sr);
        public virtual bool inShadow(ShadeRec sr, Ray ray) { return false; }
        public virtual Vect3D getDirection(ShadeRec sr)
        {
            return new Vect3D(0, 0, 0);
        }
        public virtual bool castsShadows() { return shadows; }
        
    }
}
