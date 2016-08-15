//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    sealed class GlobalVars
    {

        public const float K_EPSILON = 0.0e-6f; // very small value to prevent salt and pepper noise
        public const float SHAD_K_EPSILON = 0.1f; // very small value to prevent salt and pepper noise, specific to shaders
        public const float K_HUGE_VALUE = 1.0e6f; // very large value 
        public const float INVERSE_PI = 1.0f / (float)Math.PI; // inverse PI, speeds up calculations involving division by PI

        // Global variables set by command line parameters
        static public bool verbose = true; // verbose output in terminal while loading scene
        static public string inFile = null; // input XML file
        static public string outFile = null; // output BMP file
        
        // Frequently referenced colors stored as constants
        static public readonly RGBColor COLOR_BLACK = new RGBColor(0, 0, 0); // black
        static public readonly RGBColor COLOR_RED = new RGBColor(1.0f, 0, 0); // red
        static public readonly int H_RES = 800; // default horizontal resolution
        static public readonly int V_RES = 600; // default vertical resolution
        static public readonly int FRAGMENT_SIZE = 64; // default scene fragment size
        static public int NUM_SAMPLES; // samples per pixel. set as constant 
        static public Sampler VIEWPLANE_SAMPLER; // reference to viewplane sampler
        static public World WORLD_REF; // reference to the world entity

        // Animation related global variables
		static public bool should_close = false; // flag for if rendering should end
        static public int frameno = 0; // current frame number
    }
}
