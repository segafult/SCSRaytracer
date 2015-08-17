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

namespace RayTracer
{
    sealed class GlobalVars
    {
        public const float kEpsilon = 0.0e-6f;
        public const float shadKEpsilon = 0.00001f;
        public const float kHugeValue = 1.0e6f;
        public const float invPI = 1.0f / (float)Math.PI;

        static public bool verbose = true;
        static public string inFile = null;
        static public string outFile = null;
        

        static public readonly RGBColor color_black = new RGBColor(0, 0, 0);
        static public readonly RGBColor color_red = new RGBColor(1.0f, 0, 0);
        static public readonly int hres = 800;
        static public readonly int vres = 600;
        static public int num_samples;
        static public Sampler vp_sampler;
        static public World worldref;

		static public bool should_close = false;

        //static public float cam_zcoord = 500;
        //static public float lookat_zcoord= 0;

        //static public string path = "E:\\renderimages\\";
        //static public int frameno = 0;
    }
}
