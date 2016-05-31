//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    sealed class ViewPlane
    {
        private int _horizontalRes;
        private int _verticalRes;
        private float _pixelSize;
        private float _gamma;
        private float _inverseGamma;
        private int _maxDepth;
        private int _numSamples;
        private Sampler _viewPlaneSampler;

        // accessors
        public int HorizontalResolution
        {
            get
            {
                return _horizontalRes;
            }
            set
            {
                _horizontalRes = value;
            }
        }
        public int VerticalResolution
        {
            get
            {
                return _verticalRes;
            }
            set
            {
                _verticalRes = value;
            }
        }
        public float PixelSize
        {
            get
            {
                return _pixelSize;
            }
            set
            {
                _pixelSize = value;
            }
        }
        public float Gamma
        {
            get
            {
                return _gamma;
            }
            set
            {
                _gamma = value;
                _inverseGamma = 1 / value;
            }
        }
        public float InverseGamma
        {
            get
            {
                return _inverseGamma;
            }
        }
        public int MaximumRenderDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
            }
        }
        public int NumSamples
        {
            get
            {
                return _numSamples;
            }
            set
            {
                _numSamples = value;
            }
        }
        public Sampler ViewPlaneSampler
        {
            get
            {
                return _viewPlaneSampler;
            }
            set
            {
                _viewPlaneSampler = value;
            }
        }

        public ViewPlane()
        {

        }
    }
}