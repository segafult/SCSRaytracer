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
    abstract public class BRDF
    {
        protected Sampler sampler_ptr;
        protected Normal normal;

        virtual public RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            return GlobalVars.color_black;
        }
        virtual public RGBColor sample_f(ShadeRec sr, ref Vect3D  wi, ref Vect3D wo)
        {
            return GlobalVars.color_black;
        }
        virtual public RGBColor rho(ShadeRec sr, Vect3D wo)
        {
            return GlobalVars.color_black;
        }
    }
}
