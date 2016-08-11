//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    /// <summary>
    /// Shader for simple phong model of specular reflection (sum of ambient, diffuse and specular components)
    /// </summary>
    class PhongShader : Material
    {
        protected Lambertian ambientBRDF;
        protected Lambertian diffuseBRDF;
        protected GlossySpecular specularBRDF;

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
        public float SpecularReflectionCoefficient
        {
            set
            {
                specularBRDF.SpecularReflectionCoefficient = value;
            }
        }
        public float PhongExponent
        {
            set
            {
                specularBRDF.PhongExponent = value;
            }
        }
        public RGBColor Color
        {
            set
            {
                ambientBRDF.ColorDiffuse = value;
                diffuseBRDF.ColorDiffuse = value;
                specularBRDF.ColorSpecular = value;
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
            string toReturn = "Phong shader:\n";
            toReturn += "  ID: " + id + "\n";
            toReturn += "  Ka: " + ambientBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Kd: " + diffuseBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Cd: " + ambientBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Exp: " + specularBRDF.PhongExponent + "\n";
            toReturn += "  Ks: " + specularBRDF.SpecularReflectionCoefficient;

            return toReturn;
        }
        public PhongShader()
        {
            ambientBRDF = new Lambertian();
            diffuseBRDF = new Lambertian();
            specularBRDF = new GlossySpecular();
            PhongExponent = 200;
        }

        public PhongShader(Lambertian ambientBRDF, Lambertian diffuseBRDF, GlossySpecular specularBRDF)
        {
            this.ambientBRDF = new Lambertian(ambientBRDF);
            this.diffuseBRDF = new Lambertian(diffuseBRDF);
            this.specularBRDF = new GlossySpecular(specularBRDF);
        }

        //Gets and sets
        /*
        virtual public void setKa(float ka) { ambientBRDF.DiffuseReflectionCoefficient = ka; }
        virtual public void setKd(float kd) { diffuseBRDF.DiffuseReflectionCoefficient = kd; }
        virtual public void setCd(RGBColor c) { ambientBRDF.ColorDiffuse = c; diffuseBRDF.ColorDiffuse = c; specularBRDF.ColorSpecular = c; }
        virtual public void setExp(float exp) { specularBRDF.PhongExponent = exp; }
        virtual public void setKs(float ks) { specularBRDF.SpecularReflectionCoefficient = ks; }
        virtual public void setCd(Texture tex) { ambientBRDF.Texture = tex; diffuseBRDF.Texture = tex; }
        */

        public override RGBColor Shade(ShadeRec sr)
        {
            Vect3D reflectedDirection = -sr.Ray.Direction; //Ray pointing from point of intersection to camera.
            RGBColor L = ambientBRDF.Rho(sr, reflectedDirection) * sr.WorldPointer.AmbientLight.L(sr); //Start with ambient
            int numlights = sr.WorldPointer.LightList.Count;
            Vect3D incomingDirection; //Direction to incident light
            float nDotWi;
            bool inShadow;
            Ray shadowRay;

            //Add together light contributions for all light sources
            for(int i = 0; i<numlights; i++)
            {
                incomingDirection = sr.WorldPointer.LightList[i].getDirection(sr); //Get the direction from the point of contact to the light source.
                nDotWi = (float)(sr.Normal * incomingDirection); //Dot product of normal and light source, 0 if orthogonal, 1 if parallel.
                if(nDotWi > 0.0f)//Avoid unnecessary light summation
                {
                    inShadow = false;

                    if(sr.WorldPointer.LightList[i].castsShadows())
                    {
                        shadowRay = new Ray(sr.HitPoint+(GlobalVars.SHAD_K_EPSILON*incomingDirection), incomingDirection);
                        inShadow = sr.WorldPointer.LightList[i].inShadow(sr, shadowRay);
                    }
                    if (!inShadow)
                    {
                        //Add diffuse and specular components.
                        L += (diffuseBRDF.F(sr, reflectedDirection, incomingDirection) + specularBRDF.F(sr, reflectedDirection, incomingDirection)) * sr.WorldPointer.LightList[i].L(sr) * nDotWi;
                    }
                }
            }

            return L;
        }
    }
}
