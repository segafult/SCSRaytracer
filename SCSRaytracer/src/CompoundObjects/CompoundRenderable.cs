//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Collections.Generic;
using System.Xml;

namespace SCSRaytracer
{
    /// <summary>
    /// Compound renderable object that can serve as container class for any sort of renderable object.
    /// </summary>
    class CompoundRenderable : RenderableObject
    {
        protected List<RenderableObject> objs;

        // accessors
        public override Material Material
        {
            get
            {
                return _material;
            }
            set
            {
                int numobjs = objs.Count;
                for (int i = 0; i < numobjs; i++)
                {
                    objs[i].Material = value;
                }
            }
        }

        public CompoundRenderable()
        {
            objs = new List<RenderableObject>();
        }

        public void add_object(RenderableObject toAdd)
        {
            objs.Add(toAdd);
        }

        public override bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            float t = GlobalVars.K_HUGE_VALUE;
            Normal normal = new Normal();
            Point3D local_hit_point = new Point3D();
            bool hit = false;
            tmin = GlobalVars.K_HUGE_VALUE;
            int numobjs = objs.Count;
            Material closestmat = null;

            //Traverse the list of renderable objects, and test for collisions in the same manner as in the world
            //hit function
            for(int i = 0; i < numobjs; i++)
            {
                if(objs[i].Hit(r, ref t, ref sr) && (t<tmin))
                {
                    hit = true;
                    tmin = t;
                    closestmat = sr.ObjectMaterial;
                    normal = sr.Normal;
                    local_hit_point = sr.HitPointLocal;
                }
            }

            if(hit)
            {
                sr.TMinimum = tmin;
                sr.Normal = normal;
                sr.HitPointLocal = local_hit_point;
                sr.ObjectMaterial = closestmat;
            }

            return hit;
        }

        /*
        public override void setMaterial(Material m)
        {
            int numobjs = objs.Count;
            for(int i = 0; i < numobjs; i++)
            {
                objs[i].setMaterial(m);
            }
        }
        */

        public static CompoundRenderable LoadCompoundRenderable(XmlElement def)
        {
            CompoundRenderable toReturn = new CompoundRenderable();

            //Select all renderable children of this compound object container
            XmlNodeList children = def.SelectNodes("renderable");
            foreach(XmlElement e in children)
            {
                //Load each child and store in list
                RenderableObject rend = RenderableObject.LoadRenderableObject(e);
                if (rend != null)
                    toReturn.add_object(rend);
            }

            return toReturn;
        }
    }
}
