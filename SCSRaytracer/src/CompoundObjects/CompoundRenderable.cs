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
        protected List<RenderableObject> containedObjects;

        // accessors
        public override Material Material
        {
            get
            {
                return _material;
            }
            set
            {
                int numobjs = containedObjects.Count;
                for (int i = 0; i < numobjs; i++)
                {
                    containedObjects[i].Material = value;
                }
            }
        }

        public CompoundRenderable()
        {
            containedObjects = new List<RenderableObject>();
        }

        public void AddObject(RenderableObject toAdd)
        {
            containedObjects.Add(toAdd);
        }

        public override bool Hit(Ray r, ref float tMin, ref ShadeRec sr)
        {
            float t = GlobalVars.K_HUGE_VALUE;
            Normal normal = new Normal();
            Point3D localHitPoint = new Point3D();
            bool hit = false;
            tMin = GlobalVars.K_HUGE_VALUE;
            int countObjects = containedObjects.Count;
            Material closestObjectMaterial = null;

            //Traverse the list of renderable objects, and test for collisions in the same manner as in the world
            //hit function
            for(int i = 0; i < countObjects; i++)
            {
                if(containedObjects[i].Hit(r, ref t, ref sr) && (t<tMin))
                {
                    hit = true;
                    tMin = t;
                    closestObjectMaterial = sr.ObjectMaterial;
                    normal = sr.Normal;
                    localHitPoint = sr.HitPointLocal;
                }
            }

            if(hit)
            {
                sr.TMinimum = tMin;
                sr.Normal = normal;
                sr.HitPointLocal = localHitPoint;
                sr.ObjectMaterial = closestObjectMaterial;
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
                    toReturn.AddObject(rend);
            }

            return toReturn;
        }
    }
}
