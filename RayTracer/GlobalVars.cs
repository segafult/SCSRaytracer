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
        public const double kHugeValue = 1.0e6;

        static public readonly RGBColor color_black = new RGBColor(0, 0, 0);
        static public readonly RGBColor color_red = new RGBColor(1, 0, 0);
    }
}
