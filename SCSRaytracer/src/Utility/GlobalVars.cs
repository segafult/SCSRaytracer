//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace RayTracer
{
    sealed class GlobalVars
    {
        public const float kEpsilon = 0.0e-6f;
        public const float shadKEpsilon = 1.0f;
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
