//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;
using System.Numerics;

namespace SCSRaytracer
{
    class ThinLensCamera : Camera
    {
        Sampler depthSampler;

        float viewPlaneDistance;
        float focalPlaneDistance; //Distance to focal plane
        float radius; //Radius of the lens
        float FOVERD;

        public Sampler DepthSampler
        {
            get
            {
                return depthSampler;
            }
            set
            {
                depthSampler = value;
                depthSampler.MapSamplesToDisk();
            }
        }

        public float ViewPlaneDistance
        {
            set
            {
                viewPlaneDistance = value;
                FOVERD = focalPlaneDistance / viewPlaneDistance;
            }
        }

        public float FocalLength
        {
            set
            {
                focalPlaneDistance = value;
                FOVERD = focalPlaneDistance / viewPlaneDistance;
            }
        }

        public float Radius
        {
            set
            {
                radius = value;
            }
        }


        public ThinLensCamera () : base()
        {
            viewPlaneDistance = 250;
            focalPlaneDistance = 500;
            radius = 20;
            Up = new Vect3D(0, 1, 0);
            FOVERD = focalPlaneDistance / viewPlaneDistance;
        }

        public override void RenderScene(World w)
        {
            RGBColor L = new RGBColor();
            Ray ray = new Ray();
            ViewPlane vp = w.CurrentViewPlane;
            int depth = 0;

            Point2D sp = new Point2D();
            Point2D pp = new Point2D();
            Point2D dp = new Point2D();
            Point2D lp = new Point2D();

            //w.open_window(vp.hres, vp.vres);
            vp.PixelSize /= _zoom;

            for(int r = 0; r < vp.VerticalResolution-1; r++)
            {
                for(int c = 0; c < vp.HorizontalResolution-1; c++)
                {
                    L = GlobalVars.COLOR_BLACK;

                    for(int n = 0; n < vp.NumSamples; n++)
                    {
                        //Sample on unit square
                        sp = vp.ViewPlaneSampler.SampleUnitSquare();
                        //Sample in screenspace
                        pp.coords.X = vp.PixelSize * (c - vp.HorizontalResolution * 0.5f + sp.coords.X);
                        pp.coords.Y = vp.PixelSize * (r - vp.VerticalResolution * 0.5f + sp.coords.Y);

                        dp = depthSampler.SampleDisk();
                        lp.coords.X = dp.coords.X * radius;
                        lp.coords.Y = dp.coords.Y * radius;

                        ray.Origin = _eye + lp.coords.X * u + lp.coords.Y * v;
                        ray.Direction = GetRayDirection(pp, lp);
                        L += w.CurrentTracer.trace_ray(ray, depth);
                    }
                    L /= vp.NumSamples;
                    L *= _exposureTime;
                    w.DisplayPixel(r, c, L);
                    w.PollEvents();
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
            RGBColor L = new RGBColor();
            Ray ray = new Ray();
            ViewPlane vp = worldRef.CurrentViewPlane;
            int depth = 0;

            Point2D samplePoint = new Point2D();
            Point2D samplePointPixelSpace = new Point2D();
            Point2D samplePointOnDisk = new Point2D();
            Point2D samplePointOnLens = new Point2D();

            Sampler screenSamplerClone = GlobalVars.VIEWPLANE_SAMPLER.Clone();
            Sampler lensSamplerClone = depthSampler.Clone();

            //vp.s /= zoom;
            //Vector2 tmp2 = new Vector2(vp.hres * 0.5f, vp.vres*0.5f);
            for (int r = yCoord1; r < yCoord2; r++)
            {
                for (int c = xCoord1; c < xCoord2; c++)
                {
                    L = GlobalVars.COLOR_BLACK;

                    for (int n = 0; n < vp.NumSamples; n++)
                    {
                        //Sample on unit square
                        samplePoint = screenSamplerClone.SampleUnitSquare();
                        //Sample in screenspace
                        //Vector2 tmp1 = new Vector2(c, r);
                        //pp.coords = vp.s * (tmp1 - tmp2 * sp.coords);
                        samplePointPixelSpace.coords.X = vp.PixelSize * (c - vp.HorizontalResolution * 0.5f + samplePoint.coords.X);
                        samplePointPixelSpace.coords.Y = vp.PixelSize * (r - vp.VerticalResolution * 0.5f + samplePoint.coords.Y);

                        samplePointOnDisk = lensSamplerClone.SampleDisk();
                        samplePointOnLens.coords.X = samplePointOnDisk.coords.X * radius;
                        samplePointOnLens.coords.Y = samplePointOnDisk.coords.Y * radius;
                        //lp.coords = dp.coords * radius;

                        ray.Origin = _eye + samplePointOnLens.coords.X * u + samplePointOnLens.coords.Y * v;
                        ray.Direction = GetRayDirection(samplePointPixelSpace, samplePointOnLens);
                        L += worldRef.CurrentTracer.trace_ray(ray, depth);
                    }
                    L /= vp.NumSamples;
                    L *= _exposureTime;
                    worldRef.DisplayPixel(r, c, L);
                }
            }

            DequeueNextRenderFragment();
        }

        /// <summary>
        /// Gets the direction of a ray given a pixel and a point on a lens
        /// </summary>
        /// <param name="pixel">Pixel</param>
        /// <param name="lens">Lens</param>
        /// <returns>Normalized vector describing direction of ray</returns>
        private Vect3D GetRayDirection(Point2D pixel, Point2D lens)
        {
            Point2D p = new Point2D(pixel.coords.X*(focalPlaneDistance/viewPlaneDistance),pixel.coords.Y*(focalPlaneDistance/viewPlaneDistance));
            Vect3D dir = (p.coords.X - lens.coords.X) * u + (p.coords.Y - lens.coords.Y) * v - focalPlaneDistance * w;
            //Vector2 res = pixel.coords * FOVERD - lens.coords;
            //Vect3D dir = res.X * u + res.Y * v - f * w;
            dir.Normalize();
            return dir;
        }

        /// <summary>
        /// XML Loader function
        /// </summary>
        /// <param name="camRoot">Root element of camera tag</param>
        /// <returns>Fully assembled ThinLensCamera</returns>
        public static ThinLensCamera LoadThinLensCamera(XmlElement camRoot)
        {
            ThinLensCamera toReturn = new ThinLensCamera();

            //Set up sampler according to what's provided
            string str_sampler = camRoot.GetAttribute("sampler");
            if (!str_sampler.Equals(""))
                toReturn.DepthSampler = Sampler.LoadSampler(str_sampler);
            else
                toReturn.DepthSampler = GlobalVars.VIEWPLANE_SAMPLER;

            XmlNode node_vdp = camRoot.SelectSingleNode("vdp");
            if (node_vdp != null)
            {
                string str_vdp = ((XmlText)node_vdp.FirstChild).Data;
                float vdp = (float)Convert.ToSingle(str_vdp);
                toReturn.ViewPlaneDistance = vdp;
            }
            XmlNode node_f = camRoot.SelectSingleNode("f");
            if(node_f != null)
            {
                string str_f = ((XmlText)node_f.FirstChild).Data;
                float f = (float)Convert.ToSingle(str_f);
                toReturn.FocalLength = f;
            }
            XmlNode node_r = camRoot.SelectSingleNode("r");
            if(node_r != null)
            {
                string str_r = ((XmlText)node_r.FirstChild).Data;
                float r = (float)Convert.ToSingle(str_r);
                toReturn.Radius = r;
            }
            return toReturn;
        }
    }
}
