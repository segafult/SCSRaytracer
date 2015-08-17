//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Xml;

namespace RayTracer
{
    /// <summary>
    /// Compound renderable object that can serve as container class for any sort of renderable object.
    /// </summary>
    class CompoundRenderable : RenderableObject
    {
        protected List<RenderableObject> objs;

        public CompoundRenderable()
        {
            objs = new List<RenderableObject>();
        }

        public void add_object(RenderableObject toAdd)
        {
            objs.Add(toAdd);
        }

        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            float t = GlobalVars.kHugeValue;
            Normal normal = new Normal();
            Point3D local_hit_point = new Point3D();
            bool hit = false;
            tmin = GlobalVars.kHugeValue;
            int numobjs = objs.Count;
            Material closestmat = null;

            //Traverse the list of renderable objects, and test for collisions in the same manner as in the world
            //hit function
            for(int i = 0; i < numobjs; i++)
            {
                if(objs[i].hit(r, ref t, ref sr) && (t<tmin))
                {
                    hit = true;
                    tmin = t;
                    closestmat = sr.obj_material;
                    normal = sr.normal;
                    local_hit_point = sr.hit_point_local;
                }
            }

            if(hit)
            {
                sr.t = tmin;
                sr.normal = normal;
                sr.hit_point_local = local_hit_point;
                sr.obj_material = closestmat;
            }

            return hit;
        }

        public override void setMaterial(Material m)
        {
            int numobjs = objs.Count;
            for(int i = 0; i < numobjs; i++)
            {
                objs[i].setMaterial(m);
            }
        }

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
