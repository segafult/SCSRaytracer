//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Xml;

namespace SCSRaytracer
{
    abstract class RenderableObject
    {
        public string id;
        //public RGBColor color;
        protected Material _material;

        public virtual Material Material
        {
            get
            {
                return _material;
            }
            set
            {
                _material = value;
            }
        }
        public virtual BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox();
            }
        }

        public virtual bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            return false;
        }
        public virtual bool Hit(Ray r, float tmin)
        {
            return false;
        }
        /*
        public virtual Material getMaterial() { return _material; }
        public virtual void setMaterial(Material m)
        {
            _material = m;
        }
        
        public virtual BoundingBox get_bounding_box()
        {
            return new BoundingBox();
        }
        */
        public static RenderableObject LoadRenderableObject(XmlElement objRoot)
        {
            RenderableObject toReturn = null;
            if (objRoot.HasAttribute("type"))
            {
                string type = objRoot.GetAttribute("type");

                //Primitives loaders
                if (type.ToLower().Equals("plane"))
                    toReturn = Plane.LoadPlane(objRoot);
                else if (type.ToLower().Equals("sphere"))
                    toReturn = Sphere.LoadSphere(objRoot);
                else if (type.ToLower().Equals("torus"))
                    toReturn = Torus.LoadTorus(objRoot);
                else if (type.ToLower().Equals("triangle"))
                    toReturn = Triangle.LoadTrianglePrimitive(objRoot);
                else if (type.ToLower().Equals("box"))
                    toReturn = Box.LoadBox(objRoot);

                //Instancing and grid loaders
                else if (type.ToLower().Equals("instance"))
                    toReturn = Instance.LoadInstance(objRoot);
                else if (type.ToLower().Equals("compound"))
                    toReturn = CompoundRenderable.LoadCompoundRenderable(objRoot);
                else if (type.ToLower().Equals("grid"))
                    toReturn = UniformGrid.LoadUniformGrid(objRoot);
                else if (type.ToLower().Equals("mesh"))
                    toReturn = Mesh.LoadMesh(objRoot);
                else
                    return null;


                toReturn.id = objRoot.GetAttribute("id");
                if(objRoot.HasAttribute("mat"))
                    toReturn.Material = GlobalVars.WORLD_REF.GetMaterialByID(objRoot.GetAttribute("mat"));

                return toReturn;
            }
            else
            {
                return null;
            }
        }
    }
}
