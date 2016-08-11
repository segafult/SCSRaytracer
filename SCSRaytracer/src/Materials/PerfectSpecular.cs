//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class PerfectSpecular : BRDF
    {
        private float _reflectiveReflectionCoefficient;
        private RGBColor _colorReflection;
        private Texture _textureReflection = null;

        public float ReflectiveReflectionCoefficient
        {
            get
            {
                return _reflectiveReflectionCoefficient;
            }
            set
            {
                _reflectiveReflectionCoefficient = value;
            }
        }
        public RGBColor ColorReflection
        {
            get
            {
                return _colorReflection;
            }
            set
            {
                _colorReflection = value;
            }
        }
        public Texture TextureReflection
        {
            set
            {
                _textureReflection = value;
            }
        }

        public PerfectSpecular()
        {
            _reflectiveReflectionCoefficient = 1.0f;
            _colorReflection = new RGBColor(1.0f, 1.0f, 1.0f);
        }
        public PerfectSpecular(RGBColor color, float kr_arg)
        {
            _reflectiveReflectionCoefficient = kr_arg;
            _colorReflection = color;
        }

        public override RGBColor SampleF(ShadeRec sr, ref Vect3D incomingDirection, ref Vect3D reflectedDirection)
        {
            float nDotWo = sr.Normal * reflectedDirection;
            incomingDirection = -reflectedDirection + 2.0f * sr.Normal * nDotWo;
            if (_textureReflection == null)
                return (_reflectiveReflectionCoefficient * _colorReflection / (sr.Normal * incomingDirection));
            else
                return (_reflectiveReflectionCoefficient * _textureReflection.GetColor(sr) / (sr.Normal * incomingDirection));
        }

        /*
        public void setKr(float kr_arg) { _reflectiveReflectionCoefficient = kr_arg; }
        public void setCr(RGBColor cr_arg) { _colorReflection = cr_arg; }
        public void setCr(Texture cr_arg) { _textureReflection = cr_arg; }
        public float getKr() { return _reflectiveReflectionCoefficient; }
        public RGBColor getCr() { return _colorReflection; }
        */


    }
}
