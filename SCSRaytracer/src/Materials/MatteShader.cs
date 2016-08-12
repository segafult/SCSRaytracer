//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    /// <summary>
    /// Simple shader for matte materials
    /// </summary>
    class MatteShader : Material
    {
        private Lambertian ambientBRDF;
        private Lambertian diffuseBRDF;

        public float AmbientReflectionCoefficient
        {
            set
            {
                ambientBRDF.DiffuseReflectionCoefficient = value;
            }
        }
        public float DiffuseReflectionCoefficient
        {
            set
            {
                diffuseBRDF.DiffuseReflectionCoefficient = value;
            }
        }
        public RGBColor ColorDiffuse
        {
            set
            {
                ambientBRDF.ColorDiffuse = value;
                diffuseBRDF.ColorDiffuse = value;
            }
        }
        public Texture Texture
        {
            set
            {
                ambientBRDF.Texture = value;
                diffuseBRDF.Texture = value;
            }
        }

        public override string ToString()
        {
            string toreturn = "Matte shader:\n";
            toreturn += "  ID = " + this.id + "\n";
            toreturn += "  Ka = " + ambientBRDF.DiffuseReflectionCoefficient + "\n";
            toreturn += "  Kd = " + diffuseBRDF.DiffuseReflectionCoefficient + "\n";
            toreturn += "  Cd = " + ambientBRDF.ColorDiffuse.ToString();
            return toreturn;
        }

        public MatteShader()
        {
            ambientBRDF = new Lambertian();
            diffuseBRDF = new Lambertian();
        }

        //Setters for lambertian paramaters
        /*
        public void setKa(float ka) { ambientBRDF.DiffuseReflectionCoefficient = ka; }
        public void setKd(float kd) { diffuseBRDF.DiffuseReflectionCoefficient = kd; }
        public void setCd(RGBColor c) { ambientBRDF.ColorDiffuse = c; diffuseBRDF.ColorDiffuse = c; }
        public void setCd(Texture tex) { ambientBRDF.Texture = tex; diffuseBRDF.Texture = tex; }
        */

        public override RGBColor Shade(ShadeRec sr)
        {
            Vect3D reflectedDirection = -sr.Ray.Direction;
            RGBColor L = ambientBRDF.Rho(sr, reflectedDirection) * sr.WorldPointer.AmbientLight.GetLighting(sr);
            int numLights = sr.WorldPointer.LightList.Count;
            Vect3D incomingDirection;
            float nDotWi;
            bool inShadow;
            Ray shadowRay;
            //float t = GlobalVars.kHugeValue;
            
            //Loop through list of lights and add radiance for each diffuse light source.
            for(int i = 0; i < numLights; i++)
            {
                incomingDirection = sr.WorldPointer.LightList[i].GetDirection(sr);
                nDotWi = sr.Normal * incomingDirection;
                //Direction must not be 0,0,0 to be a diffuse light source.
                if(nDotWi > 0.0)
                {
                    inShadow = false;
                    if(sr.WorldPointer.LightList[i].CastsShadows)
                    {
                        shadowRay = new Ray(sr.HitPoint + (GlobalVars.SHAD_K_EPSILON * incomingDirection), incomingDirection);
                        inShadow = sr.WorldPointer.LightList[i].InShadow(sr, shadowRay);
                    }
                    if(!inShadow)
                    {
                        L += diffuseBRDF.F(sr, reflectedDirection, incomingDirection) * sr.WorldPointer.LightList[i].GetLighting(sr) * nDotWi;
                    }
                }
            }

            return L;
        }
    }
}
