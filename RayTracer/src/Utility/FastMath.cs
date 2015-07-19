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
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class FastMath
    {
        /// <summary>
        /// Fast approximation for the Math.Pow operation as described at
        /// http://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
        /// </summary>
        /// <param name="a">Number to be raised to an exponent</param>
        /// <param name="b">Exponent to raise a to</param>
        /// <returns>Approximation of a^b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double fastPow(double a, double b)
        {
            int x = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
            int y = (int)(b * (x - 1072632447) + 1072632447);
            return BitConverter.Int64BitsToDouble(((long)y) << 32);
        }
    }
}
