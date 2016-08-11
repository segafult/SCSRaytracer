//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class ReflectiveShader : PhongShader
    {
        private PerfectSpecular reflectiveBRDF;

        public float ReflectiveReflectionCoefficient
        {
            set
            {
                reflectiveBRDF.ReflectiveReflectionCoefficient = value;
            }
        }
        public RGBColor ColorReflection
        {
            set
            {
                reflectiveBRDF.ColorReflection = value;
            }
        }
        public Texture TextureReflection
        {
            set
            {
                reflectiveBRDF.TextureReflection = value;
            }
        }

        public ReflectiveShader() : base()
        {
            reflectiveBRDF = new PerfectSpecular();
        }

        public override string ToString()
        {
            string toReturn = "Reflective/whittal shader:\n";
            toReturn += "  ID: " + id + "\n";
            toReturn += "  Ka: " + ambientBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Kd: " + diffuseBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Cd: " + ambientBRDF.DiffuseReflectionCoefficient + "\n";
            toReturn += "  Exp: " + specularBRDF.PhongExponent + "\n";
            toReturn += "  Ks: " + specularBRDF.SpecularReflectionCoefficient +"\n";
            toReturn += "  Cr: " + reflectiveBRDF.ColorReflection.ToString() + "\n";
            toReturn += "  Kr: " + reflectiveBRDF.ReflectiveReflectionCoefficient;

            return toReturn;
        }

        public override RGBColor Shade(ShadeRec sr)
        {
            RGBColor L = base.Shade(sr); //Factor in all direct illumination

            Vect3D reflectedDirection = -sr.Ray.Direction; //Vector pointing towards camera
            Vect3D perfectReflection = new Vect3D(1.0f,1.0f,1.0f); //Vector equivalent to perfect reflection
            RGBColor fr = reflectiveBRDF.SampleF(sr, ref perfectReflection, ref reflectedDirection); //Set vectors for reflection to correct values
            Point3D hitPoint = sr.HitPoint + (perfectReflection * GlobalVars.SHAD_K_EPSILON); //Avoid salt+pepper noise
            Ray reflectedRay = new Ray(hitPoint, perfectReflection); //Cast ray from point of incidence

            L += fr * sr.WorldPointer.CurrentTracer.TraceRay(reflectedRay, sr.RecursionDepth + 1) * (sr.Normal * perfectReflection); //Recurse!

            return L; 
        }

        /*
            virtual public void setReflectivity(float refl) { ReflectiveBRDF.ReflectiveReflectionCoefficient = refl; }
            virtual public void setCr(RGBColor c)
            {
    ReflectiveBRDF.ColorReflection = c;
}
virtual public void setCr(Texture c) { ReflectiveBRDF.TextureReflection = c; }
*/
    }
}
