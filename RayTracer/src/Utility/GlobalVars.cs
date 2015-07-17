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
    sealed class GlobalVars
    {
        public const double kEpsilon = 0.0e-6;
        public const double shadKEpsilon = 0.00001;
        public const double kHugeValue = 1.0e6;
        public const double invPI = 1 / Math.PI;

        static public readonly RGBColor color_black = new RGBColor(0, 0, 0);
        static public readonly RGBColor color_red = new RGBColor(1.0, 0, 0);
    }
}
