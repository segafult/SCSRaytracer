//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class Lambertian : BRDF
    {
        private float _diffuseReflectionCoefficient;
        private RGBColor _colorDiffuse;
        private Texture _colorDiffuseTexture = null;

        public float DiffuseReflectionCoefficient
        {
            get
            {
                return _diffuseReflectionCoefficient;
            }
            set
            {
                _diffuseReflectionCoefficient = value;
            }
        }
        public RGBColor ColorDiffuse
        {
            get
            {
                return _colorDiffuse;
            }
            set
            {
                _colorDiffuse = value;
            }
        }
        public Texture Texture
        {
            set
            {
                _colorDiffuseTexture = value;
            }
        }

        public Lambertian()
        {
            _diffuseReflectionCoefficient = 0.5F;
            _colorDiffuse = new RGBColor(0.5f, 0.5f, 0.5f);
        }
        public Lambertian(float kdarg, RGBColor cdarg)
        {
            _diffuseReflectionCoefficient = kdarg;
            _colorDiffuse = cdarg;
        }
        //Copy constructor
        public Lambertian(Lambertian clone)
        {
            _diffuseReflectionCoefficient = clone.DiffuseReflectionCoefficient;
            _colorDiffuse = clone.ColorDiffuse;
        }

        /*
        public float getKd() { return _diffuseReflectionCoefficient; }
        public RGBColor getCd() { return _colorDiffuse; }
        public void setKd(float kdarg) { _diffuseReflectionCoefficient = kdarg; }
        public void setCd(RGBColor cdarg) { _colorDiffuse = cdarg; }
        public void setCd(Texture texarg) { _colorDiffuseTexture = texarg; }
        */
        
        public override RGBColor F(ShadeRec sr, Vect3D incomingDirection, Vect3D reflectedDirection)
        {
            if (_colorDiffuseTexture == null)
                return (_diffuseReflectionCoefficient * _colorDiffuse * GlobalVars.INVERSE_PI);
            else
                return (_diffuseReflectionCoefficient * _colorDiffuseTexture.GetColor(sr) * GlobalVars.INVERSE_PI);
        }
        public override RGBColor Rho(ShadeRec sr, Vect3D reflectedDirection)
        {
            if (_colorDiffuseTexture == null)
                return (_diffuseReflectionCoefficient * _colorDiffuse);
            else
                return (_diffuseReflectionCoefficient * _colorDiffuseTexture.GetColor(sr));
        }
    }
}
