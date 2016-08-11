//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    
using System;

namespace SCSRaytracer
{
    class GlossySpecular : BRDF
    {
        private float _phongExponent;
        private float _specularReflectionCoefficient;
        private RGBColor  _colorSpecular;

        public float PhongExponent
        {
            get
            {
                return _phongExponent;
            }
            set
            {
                _phongExponent = value;
            }
        }
        public float SpecularReflectionCoefficient
        {
            get
            {
                return _specularReflectionCoefficient;
            }
            set
            {
                _specularReflectionCoefficient = value;
            }
        }
        public RGBColor ColorSpecular
        {
            get
            {
                return _colorSpecular;
            }
            set
            {
                _colorSpecular = value;
            }
        }

        public GlossySpecular()
        {
            _phongExponent = 1.0f;
            _specularReflectionCoefficient = 1.0f;
            _colorSpecular = new RGBColor(1.0f, 1.0f, 1.0f);
        }
        //Copy constructor
        public GlossySpecular(GlossySpecular clone)
        {
            _phongExponent = clone.PhongExponent;
            _specularReflectionCoefficient = clone.SpecularReflectionCoefficient;
            _colorSpecular = clone.ColorSpecular;
            samplerPointer = clone.Sampler;
        }

        public override RGBColor F(ShadeRec sr, Vect3D incomingDirection, Vect3D reflectedDirection)
        {
            RGBColor L = new RGBColor(0.0f, 0.0f, 0.0f);
            float nDotWi = (sr.Normal * incomingDirection); //Dot product of normal and angle of incidence gives the angle of mirror reflection
            Vect3D r = new Vect3D(-incomingDirection + 2.0f * sr.Normal * nDotWi); //Vector describing direction of mirror reflection
            float rDotWo = (r * reflectedDirection);

            if (rDotWo > 0.0)
            {
                L = _specularReflectionCoefficient * (float)Math.Pow(rDotWo, _phongExponent) * _colorSpecular;
            }

            return L;
        }
        /*
        public void setExp(float e) { _phongExponent = e; }
        public void setKs(float ks_a) { _specularReflectionCoefficient = ks_a; }
        public void setSampler(Sampler sampler_arg) { samplerPointer = sampler_arg; }
        public void setCs(RGBColor color_arg) { _colorSpecular = color_arg; }
        public float getExp() { return _phongExponent; }
        public float getKs() { return _specularReflectionCoefficient; }
        public RGBColor getCs() { return _colorSpecular; }
        public Sampler getSampler() { return samplerPointer; }
        */


    }
}
