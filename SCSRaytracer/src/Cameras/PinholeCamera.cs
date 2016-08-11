//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace SCSRaytracer
{
    /// <summary>
    /// Simple pinhole camera with no depth of field simulation
    /// </summary>
    class PinholeCamera : Camera
    {
        private float pinholeViewPlaneDistance; //Distance between pinhole and viewplane

        public float ViewPlaneDistance
        {
            get
            {
                return pinholeViewPlaneDistance;
            }
            set
            {
                pinholeViewPlaneDistance = value;
            }
        }

        /// <summary>
        /// Default constructor, pinhole viewpane distance of 850
        /// </summary>
        public PinholeCamera() : base()
        {
            pinholeViewPlaneDistance = 850;
        }

        /// <summary>
        /// Renders the world on a single thread [deprecated, use RenderSceneMultiThreaded]
        /// </summary>
        /// <param name="world">World reference</param>
        public override void RenderScene(World worldRef)
        {
            RGBColor lightingSum;
            ViewPlane vp = worldRef.CurrentViewPlane;
            Ray ray = new Ray(_eye,new Vect3D(0,0,0));
            int depth = 0; //Depth of recursion
            Point2D sp = new Point2D(); //Sample point on a unit square
            Point2D pp = new Point2D(); ; //Sample point translated into screen space

            worldRef.OpenWindow(vp.HorizontalResolution, vp.VerticalResolution);
            vp.PixelSize /= _zoom;

            for(int row = 0; row < vp.VerticalResolution; row++)
            {
                for(int column = 0; column < vp.HorizontalResolution; column++)
                {
                    lightingSum = GlobalVars.COLOR_BLACK; //Start with no color, everything is additive

                    for(int sample = 0; sample < vp.NumSamples; sample ++)
                    {
                        sp = worldRef.CurrentViewPlane.ViewPlaneSampler.SampleUnitSquare();
                        pp.coords.X = worldRef.CurrentViewPlane.PixelSize * (column - 0.5f * vp.HorizontalResolution + sp.coords.X);
                        pp.coords.Y = worldRef.CurrentViewPlane.PixelSize * (row - 0.5f * vp.VerticalResolution + sp.coords.Y);
                        ray.Direction = GetRayDirection(pp);
                        lightingSum = lightingSum + worldRef.CurrentTracer.TraceRay(ray, depth);
                    }

                    lightingSum /= vp.NumSamples;
                    lightingSum *= _exposureTime;
                    worldRef.DisplayPixel(row, column, lightingSum);

                    //Poll events in live render view
                    worldRef.PollEvents();
                }
            }
        }

        /// <summary>
        /// Renders a single rectangular chunk of the scene
        /// </summary>
        /// <param name="worldRef">Reference to the world</param>
        /// <param name="xCoord1">Smallest x coordinate</param>
        /// <param name="xCoord2">Largest x coordinate</param>
        /// <param name="yCoord1">Smallest y coordinate</param>
        /// <param name="yCoord2">Largest y coordinate</param>
        /// <param name="threadNum">Thread number</param>
        public override void RenderSceneFragment(World worldRef, int xCoord1, int xCoord2, int yCoord1, int yCoord2, int threadNum)
        {
            //To avoid clashes with other threads accessing sampler, clone the main world sampler
            Sampler localSampler = worldRef.CurrentViewPlane.ViewPlaneSampler.Clone();

            RGBColor L;
            ViewPlane vp = worldRef.CurrentViewPlane;
            Ray ray = new Ray(_eye, new Vect3D(0, 0, 0));
            int depth = 0; //Depth of recursion
            Point2D unitSquareSample = new Point2D(); //Sample point on a unit square
            Point2D sampleInScreenSpace = new Point2D(); ; //Sample point translated into screen space
            int height = yCoord2 - yCoord1;
            int width = xCoord2 - xCoord1;

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    L = GlobalVars.COLOR_BLACK; //Start with no color, everything is additive

                    for (int sample = 0; sample < vp.NumSamples; sample++)
                    {
                        unitSquareSample = localSampler.SampleUnitSquare();
                        sampleInScreenSpace.coords.X = worldRef.CurrentViewPlane.PixelSize * (column + xCoord1 - 0.5f * vp.HorizontalResolution + unitSquareSample.coords.X);
                        sampleInScreenSpace.coords.Y = worldRef.CurrentViewPlane.PixelSize * (row + yCoord1 - 0.5f * vp.VerticalResolution + unitSquareSample.coords.Y);
                        ray.Direction = GetRayDirection(sampleInScreenSpace);
                        L = L + worldRef.CurrentTracer.TraceRay(ray, depth);
                    }

                    L /= vp.NumSamples;
                    L *= _exposureTime;

                    worldRef.DisplayPixel(row + yCoord1, column + xCoord1, L);
                }
            }

            DequeueNextRenderFragment();
        }

        /// <summary>
        /// Gets the normalized direction of a ray towards a given point starting at the origin of the camera
        /// </summary>
        /// <param name="point">Point to cast ray towards</param>
        /// <returns>Normalized direction</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vect3D GetRayDirection(Point2D point)
        {
            Vect3D dir = point.coords.X * u + point.coords.Y * v - pinholeViewPlaneDistance * w;
            dir.Normalize();
            return dir;
        }

        /// <summary>
        /// XML Load function
        /// </summary>
        /// <param name="camRoot">Root element of camera tag</param>
        /// <returns>Fully constructed PinholeCamera</returns>
        public static PinholeCamera LoadPinholeCamera(XmlElement camRoot)
        {
            PinholeCamera toReturn = new PinholeCamera();

            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                float vdp = (float)Convert.ToSingle(str_vdp);
                toReturn.ViewPlaneDistance = vdp;
            }
            return toReturn;
        }
    }
}
